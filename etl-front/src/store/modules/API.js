const API = {
    namespaced: true,
    state: {
        APIBaseUrl: 'http://localhost/GISETL'
        // APIBaseUrl: 'http://172.16.5.214/SLWDSJPT'
    },
    getters: {
        // 数据源相关
        urlGetDataSourceList: state => state.APIBaseUrl + '/DataSource/GetList', // 模型列表
        urlSaveDataSource: state => state.APIBaseUrl + '/DataSource/Save', // 保存数据源
        urlDeleteDataSource: state => state.APIBaseUrl + '/DataSource/Delete', // 删除数据源
        // 节点相关
        urlGetNodeList: state => state.APIBaseUrl + '/Node/GetList', // 节点列表
        urlSaveNode: state => state.APIBaseUrl + '/Node/Save', // 保存节点
        urlDeleteNode: state => state.APIBaseUrl + '/Node/Delete', // 删除节点
        urlGetFieldMappingsList: state => state.APIBaseUrl + '/Node/GetFieldMappingsList', // 获取字段映射列表
        urlSaveFieldMappingsList: state => state.APIBaseUrl + '/Node/SaveFieldMappingsList', // 保存字段映射列表
        // 模型相关
        urlGetModelList: state => state.APIBaseUrl + '/Model/GetList', // 模型列表
        urlSaveModel: state => state.APIBaseUrl + '/Model/Save', // 保存模型
        urlDeleteModel: state => state.APIBaseUrl + '/Model/Delete', // 删除模型
        urlCopyModel: state => state.APIBaseUrl + '/Model/Copy', // 复制模型
        urlGetStepList: state => state.APIBaseUrl + '/Model/GetStepList', // 步骤列表
        urlGetRelationList: state => state.APIBaseUrl + '/Model/GetRelationList', // 步骤关联关系列表
        // 任务相关
        urlGetTaskList: state => state.APIBaseUrl + '/Task/GetList', // 任务列表
        urlGetDynamicParams: state => state.APIBaseUrl + '/Task/GetDynamicParams', // 获取模型的动态参数
        urlSaveTask: state => state.APIBaseUrl + '/Task/Save', // 保存任务
        urlDeleteTask: state => state.APIBaseUrl + '/Task/Delete', // 删除任务
        urlSaveTodoTask: state => state.APIBaseUrl + '/Task/SaveTodo', // 保存待执行任务信息
        urlGetTodoTaskList: state => state.APIBaseUrl + '/Task/GetTodoList', // 获取待执行任务列表及其状态
        urlGetTodoTaskID: state => state.APIBaseUrl + '/Task/GetTodoID', // 根据ID从TODOLIST 表中获取数据
        urlGetTaskLog: state => state.APIBaseUrl + '/Task/GetLog', // 获取任务执行日志
        urlGetTaskLogList: state => state.APIBaseUrl + '/Task/GetLogList', // 获取任务日志
    },
    mutations: {},
    actions: {}
}

export default API