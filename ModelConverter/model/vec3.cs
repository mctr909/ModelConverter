using System;

class vec3 {
	public float x;
	public float y;
	public float z;

	public vec3(float x = 0.0f, float y = 0.0f, float z = 0.0f) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	/// <summary>ユークリッドノルム</summary>
	public float Norm { get { return (float)Math.Sqrt(this & this); } }

	/// <summary>加算</summary>
	public static vec3 operator +(vec3 a, vec3 b) {
		return new vec3(a.x + b.x, a.y + b.y, a.z + b.z);
	}
	/// <summary>減算</summary>
	public static vec3 operator -(vec3 a, vec3 b) {
		return new vec3(a.x - b.x, a.y - b.y, a.z - b.z);
	}
	/// <summary>符号反転</summary>
	public static vec3 operator -(vec3 v) {
		return new vec3(-v.x, -v.y, -v.z);
	}
	/// <summary>クロス積</summary>
	public static vec3 operator *(vec3 a, vec3 b) {
		return new vec3(
			a.y * b.z - a.z * b.y,
			a.z * b.x - a.x * b.z,
			a.x * b.y - a.y * b.x);
	}
	/// <summary>スカラー倍</summary>
	public static vec3 operator *(vec3 v, float a) {
		return new vec3(v.x * a, v.y * a, v.z * a);
	}
	/// <summary>スカラー倍</summary>
	public static vec3 operator *(float a, vec3 v) {
		return new vec3(v.x * a, v.y * a, v.z * a);
	}
	/// <summary>スカラー倍(1/a)</summary>
	public static vec3 operator /(vec3 v, float a) {
		return new vec3(v.x / a, v.y / a, v.z / a);
	}
	/// <summary>正規化後にスケーリング</summary>
	public static vec3 operator ^(vec3 v, float scale) {
		var r = v.Norm;
		if (r != 0.0f) {
			r = scale / r;
		}
		return new vec3(v.x * r, v.y * r, v.z * r);
	}
	/// <summary>内積</summary>
	public static double operator &(vec3 a, vec3 b) {
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}
}
