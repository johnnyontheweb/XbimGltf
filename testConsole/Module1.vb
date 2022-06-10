Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Xbim.Common.Geometry
Imports Xbim.GLTF
Imports Xbim.Ifc
Imports Xbim.Ifc4
Imports Xbim.ModelGeometry.Scene

Module Module1
	Sub Main()
		Dim ifc = New FileInfo(My.Application.CommandLineArgs(0))
		Dim xbim = CreateGeometry(ifc, True, False)

		Using s = IfcStore.Open(xbim.FullName)
			Dim sw As New Stopwatch()
			sw.Start()

			Dim savename = Path.ChangeExtension(s.FileName, ".gltf")
			Dim bldr = New Builder()
			Dim ret = bldr.BuildInstancedScene(s, XbimMatrix3D.Identity)
			glTFLoader.Interface.SaveModel(ret, savename)

			Debug.WriteLine($"Gltf Model exported to '{savename}' in {sw.ElapsedMilliseconds} ms.")
			Dim f As New FileInfo(s.FileName)

			' write json
			Dim jsonFileName = Path.ChangeExtension(s.FileName, "json")
			Dim bme = New Xbim.GLTF.SemanticExport.BuildingModelExtractor()
			Dim rep = bme.GetModel(s)
			rep.Export(jsonFileName)
		End Using
	End Sub

	Private Function CreateGeometry(ByVal f As FileInfo, ByVal mode As Boolean, ByVal useAlternativeExtruder As Boolean) As FileInfo
		IfcStore.ModelProviderFactory.UseHeuristicModelProvider()
		Using m = IfcStore.Open(f.FullName)
			Dim c = New Xbim3DModelContext(m)
			c.CreateContext(Nothing, mode)
			Dim newName = Path.ChangeExtension(f.FullName, mode & ".xbim")
			m.SaveAs(newName)
			Return New FileInfo(newName)
		End Using
	End Function
End Module