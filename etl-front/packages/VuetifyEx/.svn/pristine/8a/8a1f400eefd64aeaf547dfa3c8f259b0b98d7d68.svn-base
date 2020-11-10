import AddLongPressListener from './src/LongPress.js';

export default {
  name: 'VLongPress',
  install (Vue) {
    Vue.directive('longpress', {
      bind: function (el, binding, vnode) {
        AddLongPressListener(el, binding, vnode);
      }
    });
  }
}
