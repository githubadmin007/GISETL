/**
 * 模型模块
 */
import axios from '../../plugins/axios.js'

const ModelModule = {
    namespaced: true,
    state: {
        ModelList: [], // 模型列表
    },
    getters: {},
    mutations: {
        // 设置模型列表
        SetModelList (state, ModelList) {
            state.ModelList = ModelList;
        }
    },
    actions: {
        // 刷新模型列表
        RefreshModelList ({ commit, rootGetters }) {
            axios.get(rootGetters['API/urlGetModelList']).then(response => {
                commit('SetModelList', response.data);
            })
        }
    }
}

export default ModelModule;
