/*
    Copyright 2016 Esri

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.

    You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using Esri.PrototypeLab.HoloLens.Unity;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

namespace Esri.PrototypeLab.HoloLens.Demo {
    public class TerrainMap : MonoBehaviour {
        [Tooltip("Minimum distance from user")]
        public float MinimumDistance = 0.5f;

        [Tooltip("Maximum distance from user")]
        public float MaximimDistance = 10f;

        private GestureRecognizer _gestureRecognizer = null;
        private KeywordRecognizer _keywordRecognizer = null;
        private bool _isMoving = true;
        private Place _place = null;

        private const float HIT_OFFSET = 0.01f;
        private const float HOVER_OFFSET = 2f;
        private const float SIZE = 0.5f;
        private const float HEIGHT = 1f;
        private const string SPEECH_PREFIX = "show";
        private const float TERRAIN_BASE_HEIGHT = 0.02f;
        private const float VERTICAL_EXAGGERATION = 1.5f;
        private const int CHILDREN_LEVEL = 2; // 1 = Four child image tiles, 2 = Sixteen child images.

        public void Start() {
            // Listen for tap gesture.
            this._gestureRecognizer = new GestureRecognizer();
            this._gestureRecognizer.SetRecognizableGestures(
                GestureSettings.Tap |
                GestureSettings.DoubleTap
            );

            // Repond to single and double tap.
            this._gestureRecognizer.TappedEvent += (source, count, ray) => {
                switch (count) {
                    case 1:
                        if (this._isMoving) {
                            if (GazeManager.Instance.Hit) {
                                // Cease moving
                                this._isMoving = false;

                                // Stop mapping observer
                                SpatialMappingManager.Instance.StopObserver();

                                //
                                var terrain = this.transform.FindChild("terrain");
                                if (terrain == null) {
                                    // Hide footprint
                                    this.transform.FindChild("base").GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 100);

                                    // Add Terrain
                                    this.StartCoroutine(this.AddTerrain(Place.PresetPlaces[0]));
                                } else {
                                    // Restore hit test
                                    terrain.gameObject.layer = 0;
                                }
                            }
                        } else {
                            // If single tap on stationary terrain then perform reverse geocode.
                            // Exit if nothing found
                            if (!GazeManager.Instance.Hit) { return; }
                            if (GazeManager.Instance.FocusedObject == null) { return; }

                            // Exit if not terrain
                            if (GazeManager.Instance.FocusedObject.GetComponent<Terrain>() == null) { return; }

                            // Get location
                            this.StartCoroutine(this.AddStreetAddress(GazeManager.Instance.Position));
                        }
                        break;
                    case 2:
                        // Resume footprint/terrain movement.
                        if (!this._isMoving) {
                            // Set moving flag for update method
                            this._isMoving = true;

                            // Make terrain hittest invisible
                            this.GetComponentInChildren<Terrain>().gameObject.layer = 2;

                            // Resume mapping observer
                            SpatialMappingManager.Instance.StartObserver();
                        }
                        break;
                }
            };
            this._gestureRecognizer.StartCapturingGestures();

            // Listen for these phrases
            var names = Place.PresetPlaces.Select(p => {
                return string.Format("{0} {1}", SPEECH_PREFIX, p.Name);
            });
            this._keywordRecognizer = new KeywordRecognizer(names.ToArray());
            this._keywordRecognizer.OnPhraseRecognized += (e) => {
                // Exit if recognized speech not reliable.
                if (e.confidence == ConfidenceLevel.Rejected) { return; }

                string name = e.text.Substring(SPEECH_PREFIX.Length);
                Place place = Place.PresetPlaces.FirstOrDefault(p => {
                    return p.Name.ToLowerInvariant() == name.Trim().ToLowerInvariant();
                });
                if (place == null) { return; }
                this.StartCoroutine(this.AddTerrain(place));
            };
            this._keywordRecognizer.Start();

            // Create terrain footprint.
            var footprint = GameObject.CreatePrimitive(PrimitiveType.Quad);
            footprint.name = "base";
            footprint.transform.position = new Vector3(0, 0, 0);
            footprint.transform.localRotation = Quaternion.FromToRotation(
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1)
            );
            footprint.transform.localScale = new Vector3(SIZE, SIZE, 1f);
            footprint.layer = 2; // Ignore Raycast
            footprint.transform.parent = this.transform;
        }
        public void Update() {
            // Exit if moving is not enabled.
            if (!this._isMoving) { return; }

            // Exit if Gaze Manager not created.
            if (GazeManager.Instance == null) { return; }

            // Reposition terra
            this.transform.position =
                GazeManager.Instance.Hit ?
                GazeManager.Instance.Position + TerrainMap.HIT_OFFSET * GazeManager.Instance.Normal :
                Camera.main.transform.position + TerrainMap.HOVER_OFFSET * Camera.main.transform.forward;

            // *** Cannot rotate terrains! ***
            //this.transform.rotation = new Quaternion(
            //    0f,
            //    Camera.main.transform.rotation.y,
            //    0f,
            //    Camera.main.transform.rotation.w
            //);

            // Update footprint color.
            this.transform.FindChild("base").GetComponent<MeshRenderer>().material.color =
                GazeManager.Instance.Hit ? 
                new Color32(0, 255, 0, 100) :
                new Color32(255, 0, 0, 100);
        }
        private IEnumerator AddTerrain(Place place) {
            // Store current place
            this._place = place;

            // Convert lat/long to Google/Bing/AGOL tile.
            var tile = this._place.Location.ToTile(this._place.Level);

            // Get children.
            var children = tile.GetChildren(CHILDREN_LEVEL);

            // Elevation and texture variables.
            ElevationData el = null;
            Texture2D[] textures = new Texture2D[children.Length];
            yield return null;

            // Retrieve elevation.
            this.StartCoroutine(Elevation.GetHeights(tile, elevation => {
                el = elevation;
                // Construct terrain if both elevation and textures downloaded.
                if (textures.All(t => t != null)){
                    this.StartCoroutine(this.BuildTerrain(el, textures));
                }
            }));
            yield return null;

            // Retrieve imagery.
            foreach (var child in children) {
                this.StartCoroutine(Imagery.GetTexture(child, texture => {
                    textures[Array.IndexOf(children, child)] = texture;
                    // Construct terrain if both elevation and textures downloaded.
                    if (el != null && textures.All(t => t != null)) {
                        this.StartCoroutine(this.BuildTerrain(el, textures));
                    }
                }));
            }
        }
        private IEnumerator BuildTerrain(ElevationData elevation, Texture2D[] textures) {
            //
            GameObject tc = GameObject.Find("terrain");
            if (tc != null) {
                GameObject.Destroy(tc);
            }
            yield return null;

            // Center position of terrain.
            var position = this.transform.position;
            position -= new Vector3(SIZE / 2, 0, SIZE / 2);

            // Create terrain game object.
            GameObject terrainObject = new GameObject("terrain");
            terrainObject.transform.position = position;
            terrainObject.transform.parent = this.transform;
            yield return null;

            // Create terrain data.
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 33;
            terrainData.size = new Vector3(SIZE, HEIGHT, SIZE);
            terrainData.alphamapResolution =  32;
            terrainData.baseMapResolution = 1024;
            terrainData.SetDetailResolution(1024, 8);
            yield return null;

            // Tiles per side.
            var dimension = (int)Math.Sqrt(textures.Length);

            // Splat maps.
            var splats = new List<SplatPrototype>();
            foreach (var texture in textures) {
                splats.Add(new SplatPrototype() {
                    tileOffset = new Vector2(0, 0),
                    tileSize = new Vector2(
                        SIZE / dimension,
                        SIZE / dimension
                    ),
                    texture = texture
                });
                yield return null;
            }
            terrainData.splatPrototypes = splats.ToArray();
            terrainData.RefreshPrototypes();
            yield return null;

            // Get tile
            var tile = this._place.Location.ToTile(this._place.Level);

            // Construct height map.
            float[,] data = new float[
                terrainData.heightmapWidth,
                terrainData.heightmapHeight
            ];
            for (int x = 0; x < terrainData.heightmapWidth; x++) {
                for (int y = 0; y < terrainData.heightmapHeight; y++) {
                    // Scale elevation from 257x257 to 33x33
                    var x2 = Convert.ToInt32((double)x * 256 / (terrainData.heightmapWidth - 1));
                    var y2 = Convert.ToInt32((double)y * 256 / (terrainData.heightmapHeight - 1));

                    // Find index in Esri elevation array
                    var id = y2 * 257 + x2;

                    // Absolute height in map units.
                    var h1 = elevation.Heights[id];

                    // Height in model units.                        
                    var h2 = SIZE * (h1 - elevation.Min) / tile.Size;

                    // Apply exaggeration.
                    var h3 = h2 * VERTICAL_EXAGGERATION;

                    // Apply base offset.                  
                    var h4 = h3 + TERRAIN_BASE_HEIGHT;

                    // Final height.                   
                    data[terrainData.heightmapHeight - 1 - y, x] = h4;                                      
                }
            }
            terrainData.SetHeights(0, 0, data);
            yield return null;

            // Add alpha mapping
            //float[,,] maps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            var maps = new float[
                terrainData.alphamapWidth,
                terrainData.alphamapHeight,
                textures.Length
            ];
            for (int y = 0; y < terrainData.alphamapHeight; y++) {
                for (int x = 0; x < terrainData.alphamapWidth; x++) {
                    // Convert alpha coordinates into tile index. Left to right, bottom to top.
                    var tilex = x / (terrainData.alphamapWidth / dimension);
                    var tiley = y / (terrainData.alphamapHeight / dimension);
                    var index = (dimension - tiley - 1) * dimension + tilex;
                    for (int t = 0; t < textures.Length; t++) {
                        maps[y, x, t] = index == t ? 1f : 0f;
                    }
                }
            }
            terrainData.SetAlphamaps(0, 0, maps);
            yield return null;

            // Create terrain collider.
            TerrainCollider terrainCollider = terrainObject.AddComponent<TerrainCollider>();
            terrainCollider.terrainData = terrainData;
            yield return null;

            // Add terrain component.
            Terrain terrain = terrainObject.AddComponent<Terrain>();
            terrain.terrainData = terrainData;
            yield return null;

            // Calculate mesh vertices and triangles.
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Distance between vertices
            var step = SIZE / 32f;

            // Front 
            for (int x = 0; x < terrainData.heightmapWidth; x++) {
                vertices.Add(new Vector3(x * step, 0f, 0f));
                vertices.Add(new Vector3(x * step, data[0, x], 0f));
            }
            yield return null;

            // Right
            for (int z = 0; z < terrainData.heightmapHeight; z++) {
                vertices.Add(new Vector3(SIZE, 0f, z * step));
                vertices.Add(new Vector3(SIZE, data[z, terrainData.heightmapWidth - 1], z * step));
            }
            yield return null;

            // Back
            for (int x = 0; x < terrainData.heightmapWidth; x++) {
                var xr = terrainData.heightmapWidth - 1 - x;
                vertices.Add(new Vector3(xr * step, 0f, SIZE));
                vertices.Add(new Vector3(xr * step, data[terrainData.heightmapHeight - 1, xr], SIZE));
            }
            yield return null;

            // Left
            for (int z = 0; z < terrainData.heightmapHeight; z++) {
                var zr = terrainData.heightmapHeight - 1 - z;
                vertices.Add(new Vector3(0f, 0f, zr * step));
                vertices.Add(new Vector3(0f, data[zr, 0], zr * step));
            }
            yield return null;

            // Quads
            for (int i = 0; i < vertices.Count / 2 - 1; i++) {
                triangles.AddRange(new int[] {
                    2 * i + 0,
                    2 * i + 1,
                    2 * i + 2,
                    2 * i + 2,
                    2 * i + 1,
                    2 * i + 3
                });
            }

            // Create single mesh for all four sides
            GameObject side = new GameObject("side");
            side.transform.position = position;
            side.transform.parent = terrainObject.transform;
            yield return null;

            // Create mesh
            Mesh mesh = new Mesh() {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            yield return null;

            MeshFilter meshFilter = side.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            yield return null;

            MeshRenderer meshRenderer = side.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard")) {
                color = new Color32(0, 128, 128, 100)
            };
            yield return null;

            MeshCollider meshCollider = side.AddComponent<MeshCollider>();
            yield return null;
        }
        private IEnumerator AddStreetAddress(Vector3 position) {
            // Get UL and LR coordinates
            var tileUL = this._place.Location.ToTile(this._place.Level);
            var tileLR = new Tile() {
                Zoom = tileUL.Zoom,
                X = tileUL.X + 1,
                Y = tileUL.Y + 1
            };
            var coordUL = tileUL.UpperLeft;
            var coordLR = tileLR.UpperLeft;

            // Get tapped location relative to lower left.
            GameObject terrain = GameObject.Find("terrain");
            var location = position - terrain.transform.position;

            var longitude = coordUL.Longitude + (coordLR.Longitude - coordUL.Longitude) * (location.x / SIZE);
            var lattitude = coordLR.Latitude + (coordUL.Latitude - coordLR.Latitude) * (location.z / SIZE);
            var coordinate = new Coordinate() {
                Longitude = longitude,
                Latitude = lattitude
            };

            // Retrieve address.
            this.StartCoroutine(GeocodeServer.ReverseGeocode(coordinate, address => {
                // Exit if no address found.
                if (address == null) {
                    System.Diagnostics.Debug.WriteLine("No Address");
                    return;
                }

                // Create leader line.
                GameObject line = new GameObject();
                line.transform.parent = terrain.transform;
                //line.tag = "Address";

                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Standard")) {
                    color = Color.white
                };
                lineRenderer.SetWidth(0.002f, 0.002f);
                lineRenderer.SetVertexCount(2);
                lineRenderer.SetPositions(new Vector3[] {
                    position,
                    position + Vector3.up * 0.15f
                });
                lineRenderer.receiveShadows = false;
                lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
                lineRenderer.useWorldSpace = false;

                // Add text
                GameObject text = new GameObject();
                text.transform.parent = terrain.transform;
                //text.tag = "Address";
                text.transform.position = position + Vector3.up * 0.15f;
                text.transform.localScale = new Vector3(0.002f, 0.002f, 1f);

                TextMesh textMesh = text.AddComponent<TextMesh>();
                textMesh.text = address.SingleLine;
                textMesh.anchor = TextAnchor.LowerCenter;
                textMesh.fontSize = 50;
                textMesh.richText = true;
                textMesh.color = Color.white;

                Billboard billboard = text.AddComponent<Billboard>();
                billboard.PivotAxis = PivotAxis.Y;
            }));
            yield return null;
        }
    }
}
