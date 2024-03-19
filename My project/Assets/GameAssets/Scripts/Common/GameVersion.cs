using System;
using System.Runtime.Serialization;

namespace GameAsset
{
    [Serializable]
    public class GameVersion : IComparable<GameVersion>, ISerializable
    {
        public int Major = 0;
        public int Minor = 0;
        public int Build = 0;

        public GameVersion(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public GameVersion(string versionString)
        {
            string[] versionParts = versionString.Split('.');
            if (versionParts.Length == 3 && int.TryParse(versionParts[0], out int major) && int.TryParse(versionParts[1], out int minor)
                && int.TryParse(versionParts[2], out int build))
            {
                Major = major;
                Minor = minor;
                Build = build;
            }
            else
            {
                throw new ArgumentException("Invalid version string format");
            }
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }

        public static GameVersion Parse(string versionString)
        {
            return new GameVersion(versionString);
        }

        public static bool TryParse(string versionString, out GameVersion result)
        {
            result = null;
            try
            {
                result = new GameVersion(versionString);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public int CompareTo(GameVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            if (Major != other.Major)
            {
                return Major.CompareTo(other.Major);
            }

            if (Minor != other.Minor)
            {
                return Minor.CompareTo(other.Minor);
            }


            return Build.CompareTo(other.Build);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Major", Major);
            info.AddValue("Minor", Minor);
            info.AddValue("Build", Build);
        }

        public static bool operator ==(GameVersion v1, GameVersion v2)
        {
            if (ReferenceEquals(v1, v2))
            {
                return true;
            }

            if (v1 is null || v2 is null)
            {
                return false;
            }

            return v1.CompareTo(v2) == 0;
        }

        public static bool operator !=(GameVersion v1, GameVersion v2)
        {
            return !(v1 == v2);
        }

        public static bool operator >(GameVersion v1, GameVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <(GameVersion v1, GameVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >=(GameVersion v1, GameVersion v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public static bool operator <=(GameVersion v1, GameVersion v2)
        {
            return v1.CompareTo(v2) <= 0;
        }
    }
}

