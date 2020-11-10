import VContextMenu from './src/ContextMenu.vue';
import MenuManager from './src/ContextMenuManager'


VContextMenu.install = function (Vue) {
  Vue.component(VContextMenu.name, VContextMenu);
  Vue.directive('contextmenu', {
    bind (el, binding, vnode) {
      let ContextMenu = null;
      let Data = binding.value.Data;
      el.oncontextmenu = function (evt) {
        if (!ContextMenu) {
          ContextMenu = MenuManager.GetContextMenu(binding.value, vnode);
        }
        MenuManager.HideAll();
        ContextMenu.SetData(Data); // 将数据传给ContextMenu控件
        ContextMenu.ShowAt(evt.clientX, evt.clientY);
        return false;
      };
    }
  });
}

export default VContextMenu;
