import Vue from 'vue'
import Vuetify from 'vuetify'
import VuetifyEx from 'vuetify-ex'
// import VuetifyEx from '../../packages/VuetifyEx/index'
import 'vuetify/dist/vuetify.min.css'
import 'vuetify-ex/lib/VuetifyEx.css'
import HoverCol from '../components/HoverCol'

Vue.use(Vuetify);
Vue.use(VuetifyEx);
Vue.use(HoverCol);

const opts = {};
export default new Vuetify(opts);
