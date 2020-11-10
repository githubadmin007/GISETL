import axios from 'axios';
import { MessageHelper } from '../../packages/VuetifyEx/Message'

// 创建axios实例
var instance = axios.create({timeout: 1000 * 30});
// 设置post请求头
instance.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';

// 请求拦截器。每次请求前，如果存在token则在请求头中携带token
instance.interceptors.request.use(
    config => {
        // const token = store.state.UserModule.Token;
        // token && (config.headers.Authorization = token);
        return config;
    },
    error => Promise.reject(error)
)

// 响应拦截器
instance.interceptors.response.use(
    // 请求成功
    res => {
        if (res.status === 200) {
            let data = res.data;
            if (data.code === 200) {
                // data.msg && VMessage.success(data.msg);
                return Promise.resolve(data);
            }
            errorHandle(data);
        }
        return Promise.reject(res);
    },
    // 请求失败
    error => {
        const { response } = error;
        if (response) {
            // 请求已发出，但是不在2xx的范围
            console.error(response);
            // errorHandle(response.status, response.data.message);// 没想好怎么处理
            return Promise.reject(response);
        } else {
            // 处理断网的情况
            // eg:请求超时或断网时，更新state的network状态
            // network状态在app.vue中控制着一个全局的断网提示组件的显示隐藏
            // 关于断网组件中的刷新重新获取数据，会在断网组件中说明
            // store.commit('changeNetwork', false);
            tip(error.message);
        }
    }
);

// 请求失败后的错误统一处理
const errorHandle = (data) => {
    switch (data.code) {
        case 400: data.msg && tip(data.msg); break;
        case 404: tip(data.msg || '请求的资源不存在'); break;
        // case 501: tip(data.msg || '用户不存在'); toLogin(); break;
        // case 502: tip(data.msg || '用户密码错误'); toLogin(); break;
        // case 503:tip(data.msg || '未登录状态，跳转登录页'); toLogin(); break;
        default:
            // console.log('other');
    }
}

// 提示函数
const tip = msg => {
    MessageHelper.error(msg);
}

export default instance;
