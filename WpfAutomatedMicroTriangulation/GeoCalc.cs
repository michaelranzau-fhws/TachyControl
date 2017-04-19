using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Features;
using TSControl;

namespace AutomatedMicroTriangulation
{
    /// <summary>
    /// Collected mthods for calculations.
    /// </summary>
    class GeoCalc
    {
        /// <summary>
        /// Calculates the stations position out of 3 known points and their angle measurements.
        /// </summary>
        /// <param name="pointA">Left point</param>
        /// <param name="pointM">Point in the middle</param>
        /// <param name="pointB">Right point</param>
        /// <param name="angleA">Angle measurements to left point</param>
        /// <param name="angleM">Angle measurements to point in the middle</param>
        /// <param name="angleB">Angle measurements to right point</param>
        /// <returns>Stations coordinates</returns>
        public static Point3D resection(Point3D pointA, Point3D pointM, Point3D pointB, Angle angleA, Angle angleM, Angle angleB)
        {
            Point3D station = new Point3D();

            double alpha = angleM.Hz - angleA.Hz;
            double beta = angleB.Hz - angleM.Hz;
            double yC = pointA.Y + (pointA.X - pointM.X) * (1 / Math.Tan(alpha));
            double xC = pointA.X - (pointA.Y - pointM.Y) * (1 / Math.Tan(alpha));
            double yD = pointB.Y - (pointB.X - pointM.X) * (1 / Math.Tan(beta));
            double xD = pointB.X + (pointB.Y - pointM.Y) * (1 / Math.Tan(beta));
            double tDC = (yC - yD) / (xC - xD);
            double tMN = -(xC - xD) / (yC - yD);
            station.X = (float)(xD + (((pointM.Y - yD) - (pointM.X - xD) * tMN) / (tDC - tMN)));
            station.Y = (float)(yD + (station.X - xD) * tDC);
            station.Z = (float)(pointA.Z + Math.Tan(angleA.V - Math.PI / 2) * Math.Sqrt(Math.Pow(station.X - pointA.X, 2) +
                Math.Pow(station.Y - pointA.Y, 2)));

            //offset = angleA.Hz - Math.Atan((station.Y - pointA.Y) / (station.X - pointA.X));

            return station;
        }
    }
}
