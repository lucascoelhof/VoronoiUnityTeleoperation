using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityScript.Lang;

namespace RosSharp
{
    public class OccupancyGridManager : MonoBehaviour
    {
        sbyte[,] occupancyGrid;
        private Vector3 origin_position;
        private Quaternion origin_rotation;
        uint width;
        uint height;
        public float resolution;
        GameObject[,] cubes;
        GameObject parentGrid;

        int cubesRendered = 0;

        private void Start()
        {
            parentGrid = this.gameObject;
        }

        private void Update()
        {
            updateCubes();
        }

        public void updateGrid(NavigationOccupancyGrid message)
        {
            width = message.info.width;
            height = message.info.height;
            if (cubes == null)
                cubes = new GameObject[height, width];
            resolution = message.info.resolution;
            occupancyGrid = ConvertMatrix(message.data, message.info.height, message.info.width);
        }

        static sbyte[,] ConvertMatrix(sbyte[] flat, uint m, uint n)
        {
            if (flat.Length != m * n)
            {
                throw new ArgumentException("Invalid length");
            }
            sbyte[,] ret = new sbyte[m, n];
            // BlockCopy uses byte lengths: a double is 8 bytes
            Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(sbyte));
            return ret;
        }

        static sbyte[] ConvertVector(sbyte[,] matrix, uint rows, uint cols)
        {
            var array1d = new sbyte[rows * cols];
            var current = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array1d[current++] = matrix[i, j];
                }
            }
            return array1d;
        }

        private static Vector3 getPosition(GeometryPose geometryPose)
        {
            return new Vector3(
                -geometryPose.position.y,
                geometryPose.position.z,
                geometryPose.position.x);
        }

        private static Quaternion getRotation(GeometryPose geometryPose)
        {
            return new Quaternion(
                geometryPose.orientation.x,
                -geometryPose.orientation.z,
                geometryPose.orientation.y,
                geometryPose.orientation.w);
        }

        private void removeCube(int i, int j)
        {
            if (cubes[i, j] != null)
                Destroy(cubes[i, j]);
        }
        private void createCube(int i, int j, float resolution)
        {
            if (cubes[i, j] != null) return;
            Vector3 pose = new Vector3(j * resolution + resolution / 2, 0, i * resolution + resolution / 2);
            cubes[i, j] = Instantiate(Resources.Load("OccCube"), pose, Quaternion.Euler(0, 0, 0)) as GameObject;
            cubes[i, j].transform.localScale = new Vector3(resolution, 1, resolution);
            cubes[i, j].transform.SetParent(parentGrid.transform, false);
        }

        public void removeCube(float x, float y)
        {
            int i = (int)Math.Floor(x / resolution);
            int j = (int)Math.Floor(x / resolution);
            occupancyGrid[i, j] = 0;
            removeCube(i, j);
        }

        public void createCube(float x, float y)
        {
            int i = (int)Math.Floor(x / resolution);
            int j = (int)Math.Floor(x / resolution);

            if (i < 0 || i>height || j < 0 || j > width) return;

            i = (int)height - i;

            occupancyGrid[i, j] = 100;
            createCube(i, j, resolution);
        }

        public void generateNewCube(int i, int j)
        {
            if (i < 0 || i > height || j < 0 || j > width) return;

            //i = (int)height - i;

            occupancyGrid[i, j] = 100;
            //Debug.Log("New obstacle in " + i + "x" + j);
            createCube(i, j, resolution);
        }

        public void wipeOldCube(int i, int j)
        {
            if (i < 0 || i > height || j < 0 || j > width) return;

            occupancyGrid[i, j] = 0;
            removeCube(i,j);
        }

        private void updateCubes()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (occupancyGrid[i, j] >= 0 && occupancyGrid[i, j] <= 20 && cubes[i, j] != null)
                    {
                        removeCube(i, j);
                    }
                    else if ((occupancyGrid[i, j] >= 20 || occupancyGrid[i, j] == -1) && cubes[i, j] == null)
                    {
                        createCube(i, j, resolution);
                    }
                }
            }
        }

        public NavigationOccupancyGrid getOccupancyGrid()
        {
            NavigationOccupancyGrid occGrid = new NavigationOccupancyGrid();
            occGrid.info.height = this.height;
            occGrid.info.width = this.width;
            occGrid.info.resolution = this.resolution;
            occGrid.data = ConvertVector(this.occupancyGrid, this.height, this.width);
            return occGrid;
        }


    }

}
