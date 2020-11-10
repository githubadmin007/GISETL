using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using FSSG.EsriGIS.Geodatabase;
using FSSG.EsriGIS;
using System.Data.OleDb;
using ESRI.ArcGIS.esriSystem;
using System.Data;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using System.Text.RegularExpressions;

namespace FSSG.EsriGIS.Extend
{
    public static  class ITableEx
    {
        public static ITable SaveToWorkspace(this ICursor icursor, IFeatureWorkspace workspace, string layername)
        {
            UID uid = new UID();
            uid.Value = "esriGeoDatabase.Object";
            IFields newFields = new FieldsClass();
            newFields = icursor.Fields;
            ITable newtable = workspace.CreateTable(layername, newFields, uid, null, "");
            ICursor cursor = newtable.Insert(true);
            IRowBuffer prowbuffer = newtable.CreateRowBuffer();
            IRow irow = null;
            while (null != (irow = icursor.NextRow()))
            {
                for (int i = 0; i < newtable.Fields.FieldCount; i++)
                {
                    string name = irow.Value[i].ToString();
                    bool editfield = newtable.Fields.get_Field(i).Editable;
                    if (editfield)
                    {

                        prowbuffer.Value[i] = irow.Value[i];
                    }
                }
                cursor.InsertRow(prowbuffer);

            }
            cursor.Flush();
            return newtable;
        }
    }
}
