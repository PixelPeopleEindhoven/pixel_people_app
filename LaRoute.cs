using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Networking;

public class MeasurementRoot
{
    public last_measurement last_measurement;
}

[System.Serializable]
public class readings
{
    public float AmbHum, PM1, WBGT, UFP, PM25, Ozon, PM10, Temp, RelHum, AmbTemp, NO2;
}

[System.Serializable]
public class calibrated
{
    public readings readings;
}

[System.Serializable]
public class last_measurement
{
    public calibrated calibrated;
}
[System.Serializable]
public class Location
{
    public double latitude, longitude;

    public Location(double latitude, double longitude)
    {
        this.latitude  = latitude;
        this.longitude = longitude;
    }

    /// <summary>
    ///   Returns the distance in meters between this and another location.
    /// </summary>
    public double Distance(Location that)
    {
        var radEarth   = 6371000;
        var latitudeA  = that.latitude  * Math.PI / 180.0;
        var latitudeB  = this.latitude  * Math.PI / 180.0;
        var longitudeA = that.longitude * Math.PI / 180.0;
        var longitudeB = this.longitude * Math.PI / 180.0;
        var Δlongitude = (longitudeB - longitudeA);
        var Δlatitude  = (latitudeB  - latitudeA );

        var t =
            Math.Sin(Δlatitude /2) * Math.Sin(Δlatitude /2) +
            Math.Sin(Δlongitude/2) * Math.Sin(Δlongitude/2) *
            Math.Cos(latitudeA) * Math.Cos(latitudeB);
        return radEarth * (2 * Math.Atan2(Math.Sqrt(t), Math.Sqrt(1-t)));
    }

    /// <summary>
    ///   Returns true if this location is within the radius of another location.
    /// </summary>
    public bool WithinRadius(Location from, double radius)
    {
        return this.Distance(from) <= radius;
    }

    public override string ToString()
    {
        return latitude + " : " + longitude;
    }

    public Vector2 ProjectMercator(double width, double height)
    {
        var x     = (longitude+180.0) * (width / 360.0);
        var lr    = latitude * Math.PI / 180.0;
        var mercN = Math.Log(Math.Tan((Math.PI / 4) + (lr / 2.0)));
        var y     = (height / 2.0) - (width * mercN / (2.0 * Math.PI));

        return new Vector2((float)x, (float)y);
    }
}

class LocationManager
{
    public LocationManager()
    {
        Input.location.Start(1, 1);
    }

    /// <summary>
    ///   Returns the current location.
    /// </summary>
    public Location Current()
    {
        // If location services where not initialised yet or failed return a
        // mock location.
        if (Input.location.status != LocationServiceStatus.Running) {
            return new Location(-1, -1);
        }
        return new Location (
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );
    }
}

class Waypoints
{
    public Vector2 Corners { get; set; }

    Waypoint[] waypoints;

    double width  = 2000000.0;
    double height = 2000000.0;

    public Waypoints(Waypoint[] waypoints)
    {
        this.waypoints = waypoints;
        Corners        = CalcCorners();
        PlaceMapObjects();
        CylinderBetweenAllMapPoints();
    }

    Vector2 CalcCorners()
    {
        var lowestX  = double.MaxValue;
        var highestY = 0.0;

        foreach(var waypoint in waypoints) {
            var coord = waypoint.location.ProjectMercator(width, height);
            if (coord.x < lowestX ) lowestX  = coord.x;
            if (coord.y > highestY) highestY = coord.y;
        }

        return new Vector2((float)lowestX, (float)highestY);
    }

    public void PlaceMapObjects()
    {
        foreach(var waypoint in waypoints)
        {
            if (!waypoint.mapObject) continue;
            var coord = waypoint.location.ProjectMercator(width, height);
            waypoint.mapObject.transform.position = new Vector3(coord.x - Corners.x, 0.0f, coord.y - Corners.y);
        }
    }

    /// <summary>
    ///   Returns the waypoint that is closest to a certain location.
    /// </summary>
    public Waypoint Closest(Location from)
    {
        var bottom   = double.MaxValue;
        Waypoint ret = null;
        foreach (var waypoint in waypoints) {
            // Exclude waypoints that have already been visited ...
            if (waypoint.Visited) continue;
            var distance = waypoint.location.Distance(from);
            if (distance < bottom) {
                ret = waypoint;
                bottom = distance;
            }
        }
        return ret;
    }

    void CylinderBetweenObjects(GameObject a, GameObject b)
    {
        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;
        Debug.Log(a + ", " + b);
        float angle = Mathf.Atan2(posA.x - posB.x, posA.z - posB.z) * Mathf.Rad2Deg;
        Vector3 center = new Vector3((posA.x + posB.x) / 2.0f, 0.0f, (posA.z + posB.z) / 2.0f);
        float length = Vector3.Distance(posA, posB);
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = center;
        cylinder.transform.localScale = new Vector3(0.5f, length/2.0f, 0.5f); // Primitive is 2.0 long
        cylinder.transform.rotation = Quaternion.Euler(90.0f, angle, 0.0f);
    }

    void CylinderBetweenAllMapPoints()
    {
        for (int i=1; i<waypoints.Length; i++) {
            CylinderBetweenObjects(waypoints[i-1].gameObject, waypoints[i].gameObject);
        }
    }

    public Waypoint FirstEnabled()
    {
        foreach(var waypoint in waypoints)
        {
            if (!waypoint.Visited) {
                return waypoint;
            }
        }
        return null;
    }

    public void ForceNextWaypoint()
    {
        var first = FirstEnabled();
        first.Visited = true;
        first.Enabled = false;
    }
}

public class LaRoute : MonoBehaviour
{
    public Text       debugText;
    public Text       debugText2;
    public float      mapCameraSmoothingFactor = 100.0f;
    public Camera     mapCamera;
    public GameObject minimap;
    public GameObject minimapArrow;
    public AudioClip  soundClip;

    LocationManager   locationManager;
    Waypoints         waypoints;
    Vector2           corners;
    Waypoint          closest;

    int               currentWaypoint = 0;
    public Button nextButton;

    double width  = 2000000.0;
    double height = 2000000.0;

    public Text readingsTable;
    public Text correctReading;
    public AudioClip soundToPlay;

    private AudioSource source;


    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        nextButton.onClick.AddListener(delegate() {
                waypoints.ForceNextWaypoint();
            });
        locationManager = new LocationManager();

        List<Waypoint> _waypoints = new List<Waypoint>(GameObject.FindObjectsOfType<Waypoint>());
        _waypoints.Sort(delegate(Waypoint go1, Waypoint go2) {
                return go1.transform.GetSiblingIndex().CompareTo(go2.transform.GetSiblingIndex());
            });
        waypoints       = new Waypoints(_waypoints.ToArray());
        corners         = waypoints.Corners;
        StartCoroutine(GetAireasData());
    }

    // Enable the closest waypoint if it is within it's radius of the current location
    void EnableWaypointWithinRange()
    {
        var oldClosest = closest;
        var current  = locationManager.Current();
//        closest      = waypoints.Closest(current);
        closest      = waypoints.FirstEnabled();
        var isWithin = closest ? closest.location.WithinRadius(current, closest.radius) : false;
        var distance = closest.location.Distance(current);
        if (debugText) debugText.text = closest + ", " + Math.Ceiling(distance).ToString();
        if (isWithin)  {
                closest.Enabled = true;
                // source.PlayOneShot(soundToPlay);
                // Handheld.Vibrate();
        }
    }

    void RotateMinimap()
    {
        if (!minimap) return;

        Input.compass.enabled = true;

        var rotation   = minimap.transform.rotation;
        var heading    = Input.compass.trueHeading;
        var smoothness = mapCameraSmoothingFactor * Time.deltaTime;
        var quaternion = Quaternion.Euler(0, 0, heading);

        minimap.transform.rotation = Quaternion.RotateTowards(rotation, quaternion, smoothness);
    }

    void MoveMinimapCamera()
    {
        if (!mapCamera) return;

        Vector2 newPos = locationManager.Current().ProjectMercator(width, height);
        mapCamera.transform.position = new Vector3(newPos.x - corners.x, 100.0f, newPos.y - corners.y);

    }

    void RotateArrowTowards(Location location)
    {
        if (!minimapArrow) return;

        var current  = locationManager.Current();
        var coordA   = location.ProjectMercator(width, height);
        var coordB   = current.ProjectMercator (width, height);
        var α  = Math.Atan2(coordB.y - coordA.y, coordB.x - coordA.x) * Mathf.Rad2Deg;

        minimapArrow.transform.localRotation = Quaternion.Euler(0, 0, (float)α + 90);

    }

    IEnumerator<UnityWebRequestAsyncOperation> GetAireasData()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://data.aireas.com/api/v2/airboxes/3");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log("Failed to get Aireas data!");
        } else {
            string result = www.downloadHandler.text;
            MeasurementRoot m = JsonUtility.FromJson<MeasurementRoot>(result);
            Debug.Log(result);
            readings r = m.last_measurement.calibrated.readings;
            Debug.Log(m.last_measurement.calibrated.readings.NO2.ToString());
            readingsTable.text = "" +
                "AmbHum: " + r.AmbHum + "\n" +
                "PM1: " + r.PM1 + "\n" +
                "WBGT: " + r.WBGT + "\n" +
                "UFP: " + r.UFP + "\n" +
                "PM25: " + r.PM25 + "\n" +
                "Ozon: " + r.Ozon + "\n" +
                "PM10: " + r.PM10 + "\n" +
                "Temp: " + r.Temp + "\n" +
                "RelHum: " + r.RelHum + "\n" +
                "AmbTemp: " + r.AmbTemp + "\n" +
                "NO2: " + r.NO2;
            correctReading.text = r.NO2.ToString();
        }
    }

    float period = 0.0f;
    void Update()
    {
        RotateMinimap();
        // Do each second ...
        if (period > 1.0f) {
            period = 0;
            MoveMinimapCamera();
            EnableWaypointWithinRange();
            // FIXME: Broken:
            if (closest) RotateArrowTowards(closest.location);
        }
        period += Time.deltaTime;
    }

}
