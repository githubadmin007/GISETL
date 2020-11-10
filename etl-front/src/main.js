import Vue from 'vue'
import AsyncComputed from 'vue-async-computed'
import App from './App.vue'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'
import axios from './plugins/axios'

Vue.config.productionTip = false
Vue.prototype.$axios = axios

Vue.use(AsyncComputed)

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
