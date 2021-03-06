using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;

namespace Aperture3D.Graphics
{
	public static class RenderableFactory
	{
		public static MobileMesh GenerateCollisionMesh(IRenderable r)
		{
			return GenerateCollisionMesh(r, 0);
		}
		
		public static MobileMesh GenerateCollisionMesh(IRenderable r, float mass)
		{
			var Vertices = r.GetVertices();
			
			Vector3D[] verts = new Vector3D[Vertices.Length/3];
			
			for(int c = 0; c < verts.Length; c++)
			{
				verts[c] = new Vector3D(Vertices[c * 3], Vertices[c * 3 + 1], Vertices[c * 3 + 2]);	
			}
			
			if(mass != 0)return new MobileMesh(verts, r.GetIndices(), new AffineTransform(new Vector3D(5,5,5),BEPUQuaternion.Identity, Vector3D.Zero), BEPUphysics.CollisionShapes.MobileMeshSolidity.DoubleSided, mass);
			else return new MobileMesh(verts, r.GetIndices(), new AffineTransform(new Vector3D(5,5,5), BEPUQuaternion.Identity, Vector3D.Zero),BEPUphysics.CollisionShapes.MobileMeshSolidity.DoubleSided);
		}
		
		public static Renderable LoadModel(string filename)
		{
			Renderable r = new Renderable();
			Model m = ModelLoader.Load(VFS.GenerateRealPath(filename));
			
			r.Vertices = m.Vertices.ToArray();
			r.Indices = m.Indices.ToArray();
			if(m.Normals != null)r.Normals = m.Normals.ToArray();
			if(m.TexCoords != null)r.TexCoords = m.TexCoords.ToArray();
			
			return r;
			//return CalculateTangents(r);
		}
		
		public static Renderable CreatePlane(int width, int height)
		{
			Renderable r = new Renderable();
			
			List<float> vertices = new List<float>();
			List<float> texcoords = new List<float>();
			List<ushort> indices = new List<ushort>();
			List<float> normals = new List<float>();
			
			height++;
			width++;
			
			float unitWidth = 1f/(float)width;
			float unitHeight = 1f/(float)height;
			
			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					vertices.Add(x);
					vertices.Add(0);	//Height of 0
					vertices.Add(y);
					
					if(y < height - 1 && x < width - 1){
						
					indices.Add((ushort) ( (width * y)        + x) );
					indices.Add((ushort) ( (width * (y + 1))  + x) );
					indices.Add((ushort) ( (width * y)        + (x + 1)) );
						
					indices.Add((ushort) ( (width * (y + 1))        + x) );
					indices.Add((ushort) ( (width * (y + 1))  + x + 1) );
					indices.Add((ushort) ( (width * y)        + (x + 1)) );
						
					}
					
					texcoords.Add((float)x/ (float)(width - 1));
					texcoords.Add((float)y/ (float)(height - 1));
					
					normals.Add(0);
					normals.Add(1);
					normals.Add(0);
					
				}
			}
			
			r.Vertices = vertices.ToArray();
			r.Indices = indices.ToArray();
			r.Normals = normals.ToArray();
			r.TexCoords = texcoords.ToArray();
			
			//if(texcoords.Contains(1))throw new Exception();
			
			return CalculateTangents(r);
		}
		
		private static Renderable CalculateTangents(Renderable r)
		{
			List<float> tangents = new List<float>();
			
			float[] tanFinal = new float[r.Vertices.Length];
			
			for(ushort i = 0; i < r.Indices.Length; i+=3)
			{
				Vector3 v1 = new Vector3(r.Vertices[r.Indices[i] * 3], r.Vertices[r.Indices[i] * 3 + 1], r.Vertices[r.Indices[i] * 3 + 2]);
				Vector3 v2 = new Vector3(r.Vertices[r.Indices[i + 1] * 3], r.Vertices[r.Indices[i+1] * 3 + 1], r.Vertices[r.Indices[i+1] * 3 + 2]);
				Vector3 v3 = new Vector3(r.Vertices[r.Indices[i + 2] * 3], r.Vertices[r.Indices[i+2] * 3 + 1], r.Vertices[r.Indices[i+2] * 3 + 2]);
				
				Vector2 tex1 = new Vector2(r.TexCoords[r.Indices[i] * 2], r.TexCoords[r.Indices[i] * 2 + 1]);
				Vector2 tex2 = new Vector2(r.TexCoords[r.Indices[i+1] * 2], r.TexCoords[r.Indices[i+1] * 2 + 1]);
				Vector2 tex3 = new Vector2(r.TexCoords[r.Indices[i+2] * 2], r.TexCoords[r.Indices[i+2] * 2 + 1]);
				
				Vector3 e1 = v2 - v1;
				Vector3 e2 = v3 - v1;
				
				float du1 = tex2.X - tex1.X;
				float dv1 = tex2.Y - tex1.Y;
				float du2 = tex3.X - tex1.X;
				float dv2 = tex3.Y - tex1.Y;
				
				float f = 1.0f/(du1 * dv2 - du2 * dv1);
				
				Vector3 Tangent = new Vector3(f * (dv2 * e1.X - dv1 * e2.X), 
				                      f * (dv2 * e1.Y - dv1 * e2.Y),
				                      f * (dv2 * e1.Z - dv1 * e2.Z));
				
				/*tangents.Add(Tangent.X);
				tangents.Add(Tangent.Y);
				tangents.Add(Tangent.Z);
				
				tangents.Add(Tangent.X);
				tangents.Add(Tangent.Y);
				tangents.Add(Tangent.Z);
				
				tangents.Add(Tangent.X);
				tangents.Add(Tangent.Y);
				tangents.Add(Tangent.Z);*/
				
				tanFinal[r.Indices[i] * 3] = Tangent.X;
				tanFinal[r.Indices[i] * 3 + 1] = Tangent.Y;
				tanFinal[r.Indices[i] * 3 + 2] = Tangent.Z;
				
				tanFinal[r.Indices[i + 1] * 3] = Tangent.X;
				tanFinal[r.Indices[i + 1] * 3 + 1] = Tangent.Y;
				tanFinal[r.Indices[i + 1] * 3 + 2] = Tangent.Z;
				
				tanFinal[r.Indices[i + 2] * 3] = Tangent.X;
				tanFinal[r.Indices[i + 2] * 3 + 1] = Tangent.Y;
				tanFinal[r.Indices[i + 2] * 3 + 2] = Tangent.Z;
			}
			
			//r.tangents = tangents.ToArray();
			r.tangents = tanFinal;
			return r;
		}
	}
}

