/**
 *任务日志模块
 */
import axios from '../../plugins/axios.js'

const TaskLogModule = {
    namespaced: true,
    state: {
        TaskLogList: [], // 节点列表
    },
    getters: {},
    mutations: {
        // 设置节点列表
        SetTaskLogList (state, TaskLogList) {
            state.TaskLogList = TaskLogList;
        }
    },
    actions: {
        // 刷新节点列表
        RefreshTaskLogList ({ commit, rootGetters }) {
            axios.get(rootGetters['API/urlGetTaskLogList']).then(response => {
                commit('SetTaskLogList', response.data);
            })
        }
    }
}

export default TaskLogModule;
