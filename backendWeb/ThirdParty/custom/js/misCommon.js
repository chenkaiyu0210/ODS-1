if (!Number.prototype.currency) {
    Number.prototype.currency = function () {
        var parts = this.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join(',');
    };
}
$(function () {
    ///sidebar RWD Control
    $("#sidebarToggle, #sidebarToggleTop").on('click', function (e) {
        $("body").toggleClass("sidebar-toggled");
        $(".sidebar").toggleClass("toggled");
        if ($(".sidebar").hasClass("toggled")) {
            $('.sidebar .collapse').collapse('hide');
        };
    });
    //$('body').on('mousedown', function () {
        
    //    if (!$(".sidebar").hasClass("toggled")) {
    //        $(".sidebar").toggleClass("toggled");
    //    }
    //})
    
        
    ///END sidebar RWD Control
    ///bind Menu Active
    var element = $('ul.navbar-nav.sidebar a').filter(function () {        
        var _aLink = this.href.split('/')[this.href.split('/').length - 1];
        if (_aLink.indexOf('#') > -1) { return false; }
        var _href = this.href.replace('/' + _aLink, '');
        return window.location.href.indexOf(_href) > -1;
    }).addClass('active').parent();

    while (true) {
        ///完成後需變更element，否則會進入無窮迴圈
        if (element.is('li')) {
            element = element.addClass('active').parent();
        }
        else if (element.is('div')) {
            element = element.parent().addClass('show').prev();
            element.removeClass('collapsed').parent().addClass('active');
        }
        else {
            break;
        }
    }
    ///END bind Menu Active
});

function paddingLeft(str, lenght) {
    if (str.length >= lenght)
        return str;
    else
        return paddingLeft("0" + str, lenght);
}
function paddingRight(str, lenght) {
    if (str.length >= lenght)
        return str;
    else
        return paddingRight(str + "0", lenght);
}
///DataTable配置  
var dataTableSettings = { 
    "sProcessing": "正在獲取數據，請稍後...",
    "sLengthMenu": "顯示 _MENU_ 筆",
    "sZeroRecords": "沒有您要搜索的內容",
    "sInfo": "從 _START_ 到  _END_ 筆記錄 總記錄數爲 _TOTAL_ 筆",
    "sInfoEmpty": "筆數爲0",
    "sInfoFiltered": "(全部記錄數 _MAX_ 筆)",
    "sInfoPostFix": "",
    "sSearch": "搜索",
    "sUrl": "",
    "oPaginate": {
        "sFirst": "第一頁",
        "sPrevious": "上一頁",
        "sNext": "下一頁",
        "sLast": "最後一頁"
    }
}
var duallistboxSetting = {
    nonSelectedListLabel: '未選擇的組',
    selectedListLabel: '已選擇的組',
    filterTextClear: '展示所有',
    filterPlaceHolder: '過濾搜尋',
    moveSelectedLabel: "新增",
    moveAllLabel: '新增所有',
    removeSelectedLabel: "移除",
    removeAllLabel: '移除所有',
    infoText: '共{0}個組',
    infoTextFiltered: '搜尋到{0}個組 ,共{1}個組',
    infoTextEmpty: '列表為空'
}