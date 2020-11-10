using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FSSG.EsriGIS.Geodatabase
{
    public class XWorkspace
    {
        #region 打开IWorkspace
        /// <summary>
        /// 获取Access工作空间
        /// </summary>
        /// <param name="mdbPath">Access文件路径</param>
        /// <returns></returns>
        public static IWorkspace GetAccessWorkspace(string mdbPath)
        {
            if (string.IsNullOrWhiteSpace(mdbPath) || !File.Exists(mdbPath))
            {
                throw new Exception(string.Format("MDB文件不存在,文件路径：“{0}”", mdbPath));
            }
            else {
                try
                {
                    IWorkspaceFactory pWorkspaceFac = new AccessWorkspaceFactory();
                    return pWorkspaceFac.OpenFromFile(mdbPath, 0);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("读取MDB文件失败,文件路径：“{0}”", mdbPath), ex);
                }
            }
        }
        /// <summary>
        /// 获取文件地理空间GDB数据库
        /// </summary>
        /// <param name="gdbPath"></param>
        /// <returns></returns>
        public static IWorkspace GetFileGDBWorkspace(string gdbPath)
        {
            if (string.IsNullOrWhiteSpace(gdbPath) || !Directory.Exists(gdbPath))
            {
                throw new Exception(string.Format("GDB文件不存在,文件路径：“{0}”", gdbPath));
            }
            else {
                try
                {
                    //IWorkspaceFactory pWorkspaceFac = new FileGDBWorkspaceFactory();
                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    FileGDBWorkspaceFactory pWorkspaceFac = obj as FileGDBWorkspaceFactory;
                    IWorkspace workspace = pWorkspaceFac.OpenFromFile(gdbPath, 0) ;

                    return workspace;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("读取GDB文件失败,文件路径：“{0}”", gdbPath), ex);
                }
            }
        }
        /// <summary>
        /// 获取SDE数据库工作空间
        /// </summary>
        /// <param name="server">服务器名称或者IP</param>
        /// <param name="instance">数据库实例名</param>
        /// <param name="database">SDE数据库名称</param>
        /// <param name="user">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="version">连接版本</param>
        /// <returns></returns>
        public static IWorkspace GetSdeWorkspace(string server, string instance, string database, string user, string password, string version = "sde.DEFAULT")
        {
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("缺少必要参数，无法打开SDE工作空间");
            }
            else {
                //SDE数据库采用直连方式，必须加上“sde:oracle11g:”前缀；并且要连上目标机，必须加上服务器名称或者IP
                if (string.IsNullOrWhiteSpace(instance)) instance = string.IsNullOrWhiteSpace(server) ? "sde:oracle11g:orcl" : string.Format("sde:oracle11g:{0}/orcl", server);//sde:oracle11g:orcl为连接oracle client配置的主机数据库
                else if (!instance.StartsWith("sde:oracle11g:", StringComparison.OrdinalIgnoreCase)) instance = string.IsNullOrWhiteSpace(server) ? "sde:oracle11g:" + instance : string.Format("sde:oracle11g:{0}/{1}", server, instance);
                try
                {
                    //sde数据库连接属性设置
                    IPropertySet pProperty = new PropertySet();
                    pProperty.SetProperty("Server", server);//服务器名称或者IP
                    pProperty.SetProperty("Instance", instance);
                    pProperty.SetProperty("Database", database);//sde数据库名称
                    pProperty.SetProperty("User", user);//用户名称
                    pProperty.SetProperty("Password", password);//用户密码
                    pProperty.SetProperty("Version", version);//连接版本
                    return GetSdeWorkspace(pProperty);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("打开SDE数据库失败"), ex);
                }
            }
        }
        /// <summary>
        /// 获取SDE数据库工作空间
        /// </summary>
        /// <param name="propertySet">数据库连接信息</param>
        /// <returns></returns>
        private static IWorkspace GetSdeWorkspace(IPropertySet propertySet)
        {
            if (propertySet == null)
            {
                throw new Exception(string.Format("参数IPropertySet不能为空"));
            }
            try
            {
                IWorkspaceFactory2 wf = new SdeWorkspaceFactory() as IWorkspaceFactory2;
                IWorkspace workspace = wf.Open(propertySet, 0);
                return workspace;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("打开SDE数据库失败"), ex);
            }
        }
        /// <summary>
        /// 获取SDE数据库工作空间
        /// </summary>
        /// <param name="sdePath">.sde文件</param>
        /// <returns></returns>
        public static IWorkspace GetSdeWorkspace(string sdePath)
        {
            if (sdePath == "" || !sdePath.EndsWith(".sde") || !File.Exists(sdePath))
            {
                throw new Exception(string.Format("请选择sde文件"));
            }
            try
            {
                IWorkspaceFactory2 wf = new SdeWorkspaceFactory() as IWorkspaceFactory2;
                IWorkspace workspace = wf.OpenFromFile(sdePath, 0);
                return workspace;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("打开SDE数据库失败"), ex);
            }
        }
        /// <summary>
        /// 获取Shapefile数据工作空间
        /// </summary>
        /// <param name="shpPath"></param>
        /// <returns></returns>
        public static IWorkspace GetShapefileWorkspace(string shpPath)
        {
            if (string.IsNullOrWhiteSpace(shpPath) || !File.Exists(shpPath))
            {
                throw new Exception(string.Format("SHP文件不存在,文件路径：“{0}”", shpPath));
            }
            else {
                try
                {
                    IWorkspaceFactory pWorkspaceFac = new ShapefileWorkspaceFactory();
                    string directoryPath = Directory.GetParent(shpPath).FullName;
                    IWorkspace workspace = pWorkspaceFac.OpenFromFile(directoryPath, 0);
                    return workspace;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("读取SHP文件失败,文件路径：“{0}”", shpPath), ex);
                }
            }
        }
        /// <summary>
        /// 获取影像数据工作空间
        /// </summary>
        /// <param name="rasterFolder">影像文件所在文件夹</param>
        /// <returns></returns>
        public static IWorkspace GetRasterWorkspace(string rasterFolder)
        {
            if (string.IsNullOrWhiteSpace(rasterFolder) || !Directory.Exists(rasterFolder))
            {
                throw new Exception(string.Format("文件夹不存在,文件路径：“{0}”", rasterFolder));
            }
            else {
                try
                {
                    IWorkspaceFactory pWorkspaceFac = new RasterWorkspaceFactory();
                    IWorkspace workspace = pWorkspaceFac.OpenFromFile(rasterFolder, 0);
                    return workspace;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("读取文件夹失败,文件夹路径：“{0}”", rasterFolder), ex);
                }
            }
        }
        /// <summary>
        /// 获取CAD数据工作空间
        /// </summary>
        /// <param name="cadPath"></param>
        /// <returns></returns>
        public static IWorkspace GetCADWorkspcae(string cadPath)
        {
            if (string.IsNullOrWhiteSpace(cadPath) || !File.Exists(cadPath))
            {
                throw new Exception(string.Format("CAD文件不存在,文件路径：“{0}”", cadPath));
            }
            else {
                try
                {
                    IWorkspaceFactory pWorkspaceFac = new CadWorkspaceFactory();
                    IWorkspace workspace = pWorkspaceFac.OpenFromFile(cadPath, 0);
                    return workspace;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("读取CAD文件失败,文件路径：“{0}”", cadPath), ex);
                }
            }
        }
        #endregion

        /// <summary>
        /// 创建GDB
        /// </summary>
        /// <param name="gdbPath">gdb路径</param>
        /// <param name="cover">是否覆盖</param>
        /// <param name="open">是否打开并返回工作空间</param>
        /// <returns></returns>
        public static IWorkspace CreateGDB(string gdbPath,bool cover = false, bool open = true) {
            if (Directory.Exists(gdbPath) && cover) {
                Directory.Delete(gdbPath, true);
            }
            FileGDBWorkspaceFactoryClass fac = new FileGDBWorkspaceFactoryClass();
            if (!Directory.Exists(gdbPath))
            {
                fac.Create(Path.GetDirectoryName(gdbPath), Path.GetFileName(gdbPath), null, 0);
            }
            if (open)
            {
                return fac.OpenFromFile(gdbPath, 0);
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// 打开ShapeFile文件
        /// </summary>
        /// <param name="shpPath"></param>
        /// <returns></returns>
        public static IFeatureClass OpenShapeFile(string shpPath) {
            IWorkspace workspace = GetShapefileWorkspace(shpPath);
            string filename = Path.GetFileName(shpPath);
            return workspace.TryOpenFeatureClass(filename);
        }
    }
}
