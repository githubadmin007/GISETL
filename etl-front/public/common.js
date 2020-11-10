/**
 * 移除数组内的某一个元素
 * item:要移除的数组元素
 */
Array.prototype.remove = function (value) {
    var index = this.indexOf(value);
    if (index > -1) {
        this.splice(index, 1);
    }
};
/*
    筛选并移除数组内符合条件的元素
    filter:筛选条件
*/
Array.prototype.filterAndRemove = function (filter) {
    var lst = this.filter(filter);
    for (var i in lst) {
        this.remove(lst[i]);
    }
}

/*
    将时间对象格式化为字符串
    format:时间格式
*/
Date.prototype.Format = function (format) {
    // 年
    if (format.indexOf('yyyy') > -1 || format.indexOf('YYYY') > -1) {
        format = format.replace('yyyy', this.getFullYear()).replace('YYYY', this.getFullYear());
    }
    // 月
    if (format.indexOf('MM') > -1) {
        var M = this.getMonth() + 1;
        if (M < 10) {
            M = ("00" + M).substr(("" + M).length)
        }
        format = format.replace('MM', M);

        // format = format.replace('MM', this.getMonth() + 1);
    }
    // 日
    if (format.indexOf('dd') > -1) {
        var d = this.getDate();
        if (d < 10) {
            d = ("00" + d).substr(("" + d).length)
        }
        format = format.replace('dd', d);

        // format = format.replace('dd', this.getDate());
    }
    // 时
    if (format.indexOf('HH') > -1) {
        var H = this.getHours();
        if (H < 10) {
            H = ("00" + H).substr(("" + H).length)
        }
        format = format.replace('HH', H);

        // format = format.replace('HH', this.getHours());
    }
    if (format.indexOf('hh') > -1) {
        var h = this.getHours() % 12;
        if (h < 10) {
            h = ("00" + h).substr(("" + h).length)
        }
        format = format.replace('hh', h);

        // format = format.replace('hh', this.getHours() % 12);
    }
    // 分
    if (format.indexOf('mm') > -1) {
        var m = this.getMinutes();
        if (m < 10) {
            m = ("00" + m).substr(("" + m).length)
        }
        format = format.replace('mm', m);

        //format = format.replace('mm', this.getMinutes());
    }
    // 秒
    if (format.indexOf('ss') > -1) {
        var s = this.getSeconds();
        if (s < 10) {
            s = ("00" + s).substr(("" + s).length)
        }
        format = format.replace('ss', s);

        // format = format.replace('ss', this.getSeconds());
    }

    return format;
}





// 生成GUID
function guid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}
