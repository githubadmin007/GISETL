/*
  增加一个新组建需要在此文件改动三个地方：
  1.import该组件
  2.在install中调用Vue.use或添加到原型链
  3.export该组件
  注：添加时按组件名排序，方便查找
*/
import VContextMenu from './ContextMenu'
import VHoverCol from './HoverCol'
import VLongPress from './LongPress'
import VMessage, { MessageHelper } from './Message'
import VNestList from './NestList'
import VPopupWindow, { WindowManager } from './PopupWindow'

const install = function (Vue) {
  Vue.use(VContextMenu);
  Vue.use(VHoverCol);
  Vue.use(VLongPress);
  Vue.use(VMessage);
  Vue.use(VNestList);
  Vue.use(VPopupWindow);
  Vue.prototype.$VMessage = MessageHelper;
  Vue.prototype.$VWindow = WindowManager;
}

const VuetifyEx = {
  version: '0.0.1',
  install,
  VContextMenu,
  VHoverCol,
  VLongPress,
  VMessage,
  VNestList,
  VPopupWindow
}

export default VuetifyEx;

// 编译为js并在html中直接引用的情况
if (typeof window !== 'undefined' && window.Vue) {
  window.Vue.use(VuetifyEx)
}
