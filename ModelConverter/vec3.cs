using System;

class vec3 {
    public float x;
    public float y;
    public float z;

    public double Abs { get { return Math.Sqrt(this ^ this); } }

    public vec3 Norm {
        get {
            var r = (float)Math.Sqrt(this ^ this);
            if (r == 0.0f) {
                return new vec3();
            }
            return new vec3(x / r, y / r, z / r);
        }
    }

    public vec3(vec3 v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    public vec3(float x = 0.0f, float y = 0.0f, float z = 0.0f) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public static vec3 operator +(vec3 a, vec3 b) {
        return new vec3(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static vec3 operator -(vec3 a, vec3 b) {
        return new vec3(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static vec3 operator -(vec3 v) {
        return new vec3(-v.x, -v.y, -v.z);
    }
    public static vec3 operator *(vec3 a, vec3 b) {
        return new vec3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x);
    }
    public static vec3 operator *(vec3 v, float k) {
        return new vec3(v.x * k, v.y * k, v.z * k);
    }
    public static vec3 operator *(float k, vec3 v) {
        return new vec3(v.x * k, v.y * k, v.z * k);
    }
    public static vec3 operator /(vec3 v, float k) {
        return new vec3(v.x / k, v.y / k, v.z / k);
    }
    public static double operator ^(vec3 a, vec3 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public double Distance(vec3 to) {
        var sx = to.x - x;
        var sy = to.y - y;
        var sz = to.z - z;
        return Math.Sqrt(sx * sx + sy * sy + sz * sz);
    }
    public double Azimuth(vec3 to) {
        return Math.Atan2(to.z - z, to.x - x);
    }
    public double Altitude(vec3 to) {
        var sx = to.x - x;
        var sz = to.z - z;
        return Math.Atan2(to.y - y, Math.Sqrt(sx * sx + sz * sz));
    }
}
