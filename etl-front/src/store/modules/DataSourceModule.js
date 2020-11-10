/**
 * 数据源模块
 */
import axios from '../../plugins/axios.js'

const DataSourceModule = {
    namespaced: true,
    state: {
        DataSourceList: [], // 数据源列表
    },
    getters: {},
    mutations: {
        // 设置模型列表
        SetDataSourceList(state, DataSourceList) {
            state.DataSourceList = DataSourceList;
        }
    },
    actions: {
        // 刷新模型列表
        RefreshDataSourceList({ commit, rootGetters }) {
            axios.get(rootGetters['API/urlGetDataSourceList']).then(response => {
                commit('SetDataSourceList', response.data);
            })
        }
    }
}

export default DataSourceModule;
