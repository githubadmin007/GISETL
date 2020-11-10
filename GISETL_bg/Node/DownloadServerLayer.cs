using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using FSSG.EsriGIS.MapServer;
using GISETL_bg.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    public class DownloadServerLayer : _BaseNode
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        string serverUrl
        {
            get
            {
                return GetParam("server_url");
            }
        }
        /// <summary>
        /// 数据源id
        /// </summary>
        string dataSourceId
        {
            get
            {
                return GetParam("data_source_id");
            }
        }
        /// <summary>
        /// 图层名
        /// </summary>
        string layerName
        {
            get
            {
                return GetParam("layer_name");
            }
        }
        /// <summary>
        /// 是否删除旧图层
        /// </summary>
        bool deleteOld
        {
            get
            {
                return GetParam("delete_old") == "是";
            }
        }
        /// <summary>
        /// 是否清空记录
        /// </summary>
        bool clearRecord
        {
            get
            {
                return GetParam("clear_record") == "是";
            }
        }
        public DownloadServerLayer(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }
        public override bool Exexute()
        {
            ServerLayer server = new ServerLayer(serverUrl);
            IWorkspace workspace = GisDataHelper.OpenWorkspace(dataSourceId);
            Output = server.SaveToFeatureClass(workspace, layerName, deleteOld, clearRecord);
            return Output != null;
        }
    }
}
