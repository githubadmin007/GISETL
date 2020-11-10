import PopupWindow from './src/PopupWindow.vue';
import WindowManager from './src/WindowManager'

PopupWindow.install = function (Vue) {
  Vue.component(PopupWindow.name, PopupWindow);
}

export default PopupWindow;
export { WindowManager };
