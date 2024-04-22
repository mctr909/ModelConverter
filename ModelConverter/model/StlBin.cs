using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class StlBin : BaseModel {
	public override EAxisOrder AxisOrder { get { return EAxisOrder.XZY; } }
	public override EAxisDir AxisDir { get { return EAxisDir.XpYnZp; } }

	public StlBin() { }

	public StlBin(string path) {
		var arrName = new byte[80];
		var arrCount = new byte[4];
		var arrFloat = new byte[4];
		var arrReserved = new byte[2];
		using (var fs = new FileStream(path, FileMode.Open)) {
			while (fs.Position < fs.Length) {
				var curObject = new Object();
				fs.Read(arrName, 0, arrName.Length);
				fs.Read(arrCount, 0, arrCount.Length);
				curObject.Name = Encoding.UTF8.GetString(arrName).Replace("\0", "");
				var count = BitConverter.ToUInt32(arrCount, 0);
				for (int i = 0; i < count; i++) {
					var curSurface = new Surface();
					// Normal
					var curNorm = mNormList.Count;
					fs.Read(arrFloat, 0, arrFloat.Length);
					var x = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					var y = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					var z = BitConverter.ToSingle(arrFloat, 0);
					mNormList.Add(new vec3(x, y, z));

					// Vartex 1
					var idx = new Index();
					idx.Vert = mVertList.Count;
					idx.Norm = curNorm;
					fs.Read(arrFloat, 0, arrFloat.Length);
					x = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					y = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					z = BitConverter.ToSingle(arrFloat, 0);
					mVertList.Add(new vec3(x, y, z));
					curSurface.Indices.Add(idx);

					// Vartex 2
					idx = new Index();
					idx.Vert = mVertList.Count;
					idx.Norm = curNorm;
					fs.Read(arrFloat, 0, arrFloat.Length);
					x = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					y = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					z = BitConverter.ToSingle(arrFloat, 0);
					mVertList.Add(new vec3(x, y, z));
					curSurface.Indices.Add(idx);

					// Vartex 3
					idx = new Index();
					idx.Vert = mVertList.Count;
					idx.Norm = curNorm;
					fs.Read(arrFloat, 0, arrFloat.Length);
					x = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					y = BitConverter.ToSingle(arrFloat, 0);
					fs.Read(arrFloat, 0, arrFloat.Length);
					z = BitConverter.ToSingle(arrFloat, 0);
					mVertList.Add(new vec3(x, y, z));
					curSurface.Indices.Add(idx);

					// Reserved
					fs.Read(arrReserved, 0, arrReserved.Length);
					//
					curObject.Surfaces.Add(curSurface);
				}
				mObjectList.Add(curObject);
			}
		}
	}

	public override void Save(string path) {
		SwapAxis(EAxisOrder.XZY);
		ReverseAxis(EAxisDir.XpYnZp);
		ToTriangle();
		var surfList = new List<Surface>();
		foreach (var obj in mObjectList) {
			var surfCount = 0;
			foreach (var s in obj.Surfaces) {
				if (0 < s.Indices.Count) {
					surfCount++;
				}
			}
			if (0 == surfCount) {
				continue;
			}
			foreach (var s in obj.Surfaces) {
				if (0 == s.Indices.Count) {
					continue;
				}
				surfList.Add(s);
			}
		}

		var arrReserved = new byte[2];
		var fs = new FileStream(path, FileMode.Create);
		byte[] tName = new byte[] { 0 };
		var name = Path.GetFileNameWithoutExtension(path);
		if (!string.IsNullOrEmpty(name)) {
			tName = Encoding.UTF8.GetBytes(name);
		}

		var arrName = new byte[80];
		Array.Copy(tName, arrName, Math.Min(arrName.Length, tName.Length));
		fs.Write(arrName, 0, arrName.Length);
		
		var arrCount = BitConverter.GetBytes(surfList.Count);
		fs.Write(arrCount, 0, arrCount.Length);

		foreach (var s in surfList) {
			if (0 == s.Indices.Count) {
				continue;
			}
			// Normal
			var nn = new vec3();
			foreach (var idx in s.Indices) {
				if (0 <= idx.Norm) {
					var n = mNormList[idx.Norm];
					nn += n;
				}
			}
			nn ^= 1;
			var arrX = BitConverter.GetBytes(nn.x);
			var arrY = BitConverter.GetBytes(nn.y);
			var arrZ = BitConverter.GetBytes(nn.z);
			fs.Write(arrX, 0, arrX.Length);
			fs.Write(arrY, 0, arrY.Length);
			fs.Write(arrZ, 0, arrZ.Length);
			// Vertex
			foreach (var idx in s.Indices) {
				var v = mVertList[idx.Vert];
				arrX = BitConverter.GetBytes(v.x);
				arrY = BitConverter.GetBytes(v.y);
				arrZ = BitConverter.GetBytes(v.z);
				fs.Write(arrX, 0, arrX.Length);
				fs.Write(arrY, 0, arrY.Length);
				fs.Write(arrZ, 0, arrZ.Length);
			}
			// Reserved
			fs.Write(arrReserved, 0, arrReserved.Length);
		}
		fs.Close();
	}
}
