using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public class Header
{
    public string status;
}

[Serializable]
public class Icon
{
    public string url;
}

[Serializable]
public class Category
{
    public string id;
    public string ooiType;
    public string title;
    public Icon icon;
}

[Serializable]
public class Meta
{
    public Author author;
    public Source source;
    public SystemData system;
    public Timestamp timestamp;
}

[Serializable]
public class Author
{
    public string id;
    public string name;
}

[Serializable]
public class Source
{
    public string type;
    public string id;
}

[Serializable]
public class SystemData
{
    public string createdIn;
    public string lastModifiedIn;
}

[Serializable]
public class Timestamp
{
    public string createdAt;
    public string lastModifiedAt;
}

[Serializable]
public class Texts
{
    public string shortText;
    public string longText;
}

[Serializable]
public class PrimaryRegion
{
    public string type;
    public string id;
    public string title;
}

[Serializable]
public class Region
{
    public string type;
    public string id;
}

[Serializable]
public class Coordinate
{
    public string title;
    public string type;
    public string value;
}

[Serializable]
public class CommunityInfo
{
    public RatingDetails ratingDetails;
    public float rating;
    public int ratingCount;
    public int commentCount;
}

[Serializable]
public class RatingDetails
{
    public int oneStar;
    public int twoStar;
    public int threeStar;
    public int fourStar;
    public int fiveStar;
}

[Serializable]
public class GeoJson
{
    public string type;
    public Coord[] coordinates;
}

[Serializable]
public class Coord
{
    public float x;
    public float z;
    public float y;
}

[Serializable]
public class Poi
{
    public string id;
}

[Serializable]
public class Literature
{
    public string id;
}

[Serializable]
public class Season
{
    public string jan;
    public string feb;
    public string mar;
    public string apr;
    public string may;
    public string jun;
    public string jul;
    public string aug;
    public string sep;
    public string oct;
    public string nov;
    public string dec;
}

[Serializable]
public class Metrics
{
    public Duration duration;
    //public Elevation elevation;
    public long length;
}

[Serializable]
public class Duration
{
    public float minimal;
}

[Serializable]
public class Elevation
{
    public ElevationProfile elevationProfile;
    public int ascent;
    public int descent;
    public int minAltitude;
    public int maxAltitude;
}

[Serializable]
public class ElevationProfile
{
    public string url;
    public string fallbackUrl;
}

[Serializable]
public class RatingInfo
{
    public int stamina;
    public int difficulty;
    public int landscape;
    public int experience;
    public int riskPotential;
}

[Serializable]
public class WayTypeInfo
{
    public Legend[] legend;
}

[Serializable]
public class Legend
{
    public string type;
    public string title;
    public double length;
    public string color;
}

[Serializable]
public class Content
{
    
    public string type;
    public string id;
    public string title;
    //public Category category;
    //public Meta meta;
    //public Texts texts;
    //public Coord point;
    //public PrimaryRegion region;
    //public Region[] regions;
    //public Coordinate[] coordinates;
    //public CommunityInfo communityInfo; 
    public GeoJson geoJson;
    //public Poi[] pois;
    //public Literature[] literature;
    //public Season season;
    public Metrics metrics;
    public RatingInfo ratingInfo;
    //public WayTypeInfo wayTypeInfo;
    //public string teaserText;
    //public bool isWinter;
    //public string openState;
    //public bool isClosedByClosure;
    //public bool isIgnoreClosedByClosure;
}

[Serializable]
public class Route
{
    public string type;
    public Content[] contents;
}

[Serializable]
public class Root
{
    public Header header;
    public Route answer;
}

public class WanderroutenReader
{
    public static Route ReadRoute(String content)
    {
        Root data = JsonUtility.FromJson<Root>(content);
        Debug.Log(data);
        Debug.Log(data.answer);
        return data.answer;
    }
}

