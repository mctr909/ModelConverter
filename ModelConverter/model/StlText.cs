using System.IO;

class StlText : BaseModel {
	public override EAxisOrder AxisOrder { get { return EAxisOrder.XZY; } }
	public override EAxisDir AxisDir { get { return EAxisDir.XpYnZp; } }

	public StlText() { }

	public StlText(string path) {
		var curSurface = new Surface();
		var curObject = new Object();
		var curNorm = 0;
		using (var fs = new StreamReader(path)) {
			while (!fs.EndOfStream) {
				var line = fs.ReadLine().Replace("\t", " ").TrimStart().Replace("  ", " ");
				var cols = line.Split(' ');
				if (cols.Length < 1 || string.IsNullOrEmpty(cols[0])) {
					continue;
				}

				switch (cols[0].ToLower()) {
				case "solid":
					curObject = new Object();
					curObject.Name = 2 <= cols.Length ? cols[1] : "";
					break;
				case "facet":
					curSurface = new Surface();
					if (cols.Length < 2) {
						break;
					}
					if (cols[1].ToLower() == "normal") {
						curNorm = mNormList.Count;
						mNormList.Add(new vec3(float.Parse(cols[2]), float.Parse(cols[3]), float.Parse(cols[4])));
					}
					break;
				case "outer":
					break;
				case "vertex": {
					var idx = new Index();
					idx.Vert = mVertList.Count;
					idx.Norm = curNorm;
					curSurface.Indices.Add(idx);
					var v = new vec3(float.Parse(cols[1]), float.Parse(cols[2]), float.Parse(cols[3]));
					mVertList.Add(v);
					break;
				}
				case "endloop":
					break;
				case "endfacet":
					curObject.Surfaces.Add(curSurface);
					break;
				case "endsolid":
					mObjectList.Add(curObject);
					break;

				default:
					return;
				}
			}
		}
	}

	public override void Save(string path) {
		SwapAxis(EAxisOrder.XZY);
		ReverseAxis(EAxisDir.XpYnZp);
		ToTriangle();
		var fs = new StreamWriter(path);
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
			fs.WriteLine("solid \"{0}\"", obj.Name.Replace("\"", ""));
			foreach (var s in obj.Surfaces) {
				if (0 == s.Indices.Count) {
					continue;
				}
				var nn = new vec3();
				foreach (var idx in s.Indices) {
					if (0 <= idx.Norm) {
						var n = mNormList[idx.Norm];
						nn += n;
					}
				}
				nn ^= 1;
				fs.WriteLine("\tfacet normal {0} {1} {2}", nn.x, nn.y, nn.z);
				fs.WriteLine("\t\touter loop");
				foreach (var idx in s.Indices) {
					var v = mVertList[idx.Vert];
					fs.WriteLine("\t\t\tvertex {0} {1} {2}", v.x, v.y, v.z);
				}
				fs.WriteLine("\t\tendloop");
				fs.WriteLine("\tendfacet");
			}
			fs.WriteLine("endsolid");
		}
		fs.Close();
	}
}
