using Esri.PrototypeLab.HoloLens.Unity;
using HoloToolkit.Unity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Esri.PrototypeLab.HoloLens.Demo {
    public class TerrainMap : MonoBehaviour {
        [Tooltip("Minimum distance from user")]
        public float MinimumDistance = 0.5f;

        [Tooltip("Maximum distance from user")]
        public float MaximimDistance = 10f;

        private GestureRecognizer _gestureRecognizer = null;
        private bool _isMoving = true;

        private const float HIT_OFFSET = 0.01f;
        private const float HOVER_OFFSET = 2f;
        private const float SIZE = 0.5f;
        private const float HEIGHT = 0.25f;

        private GameObject _footprint = null;

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

                                //if (this._terrainObject !=null) { return; }

                                // Hide footprint
                                this._footprint.SetActive(false);

                                // Convert lat/long to Google/Bing/AGOL tile.
                                var coordinate = new Coordinate() {
                                    Longitutude = -114.738206f,
                                    Latitude = 36.015578f
                                };
                                var tile = coordinate.ToTile(12);

                                // Request tile imagery and elevation.
                                this.StartCoroutine(Imagery.GetTexture(tile, texture => {
                                    this.StartCoroutine(Elevation.GetHeights(tile, elevation => {
                                        this.BuildTerrain(texture, elevation);
                                    }));
                                }));
                            }
                        }
                        break;
                    case 2:
                        // Resume footprint/terrain movement.
                        if (!this._isMoving) {
                            this._isMoving = true;
                        }
                        break;
                }
            };
            this._gestureRecognizer.StartCapturingGestures();

            // Create terrain footprint.
            this._footprint = GameObject.CreatePrimitive(PrimitiveType.Quad);
            this._footprint.transform.position = new Vector3(0, 0, 0);
            this._footprint.transform.localRotation = Quaternion.FromToRotation(
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1)
            );
            this._footprint.transform.localScale = new Vector3(SIZE, SIZE, 1f);
            this._footprint.layer = 2; // Ignore Raycast
            this._footprint.transform.parent = this.transform;
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

            // Ensure that map is always facing the user.
            this.transform.rotation = new Quaternion(
                0f,
                Camera.main.transform.rotation.y,
                0f,
                Camera.main.transform.rotation.w
            );

            // Update footprint color.
            this._footprint.GetComponent<MeshRenderer>().material.color =
                GazeManager.Instance.Hit ? 
                new Color32(0, 255, 0, 100) :
                new Color32(255, 0, 0, 100);
        }
        private void BuildTerrain(Texture2D texture, ElevationData elevation) {
            // Center position of terrain.
            var position = this.transform.position;
            position -= new Vector3(SIZE / 2, 0, SIZE / 2);

            // Create terrain game object.
            GameObject _terrainObject = new GameObject();
            _terrainObject.transform.position = position;
            _terrainObject.transform.parent = this.transform;

            // Create terrain data
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 33;
            terrainData.size = new Vector3(SIZE, HEIGHT, SIZE);
            terrainData.alphamapResolution = 33;
            terrainData.baseMapResolution = 1024;
            terrainData.SetDetailResolution(1024, 8);
            terrainData.splatPrototypes = new SplatPrototype[] {
                new SplatPrototype() {
                    tileOffset = new Vector2(0,0),
                    tileSize = new Vector2(SIZE, SIZE),
                    texture = texture
                }
            };

            float[,] data = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
            for (int x = 0; x < terrainData.heightmapWidth; x++) {
                for (int y = 0; y < terrainData.heightmapHeight; y++) {
                    //int index = y * 257 + x;
                    //float height = elevation.Heights[index];
                    //float normalized = HEIGHT * Convert.ToSingle(height - elevation.Min) / Convert.ToSingle(elevation.Max - elevation.Min);
                    //data[y, x] = normalized;

                    var x2 = Convert.ToInt32((double)x * 256 / (terrainData.heightmapWidth - 1));
                    var y2 = Convert.ToInt32((double)y * 256 / (terrainData.heightmapHeight - 1));
                    var id = y2 * 257 + x2;
                    var h1 = elevation.Heights[id];
                    var h2 = HEIGHT * Convert.ToSingle(h1 - elevation.Min) / Convert.ToSingle(elevation.Max - elevation.Min);
                    data[32 - y, x] = h2;
                }
            }
            terrainData.SetHeights(0, 0, data);

            float[,,] maps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            for (int y = 0; y < terrainData.alphamapHeight; y++) {
                for (int x = 0; x < terrainData.alphamapWidth; x++) {
                    maps[x, y, 0] = 1;
                }
            }
            terrainData.SetAlphamaps(0, 0, maps);

            // Create terrain collider.
            TerrainCollider terrainCollider = _terrainObject.AddComponent<TerrainCollider>();
            terrainCollider.terrainData = terrainData;

            Terrain terrain = _terrainObject.AddComponent<Terrain>();
            terrain.terrainData = terrainData;
        }
    }
}
