$(function () {
    //身分證字號或外籍人士居留証驗證
    /*
     * 第一個字元代表地區，轉換方式為：A轉換成1,0兩個字元，B轉換成1,1……但是Z、I、O分別轉換為33、34、35
     * 第二個字元代表性別，1代表男性，2代表女性
     * 第三個字元到第九個字元為流水號碼。
     * 第十個字元為檢查號碼。
     * 每個相對應的數字相乘，如A123456789代表1、0、1、2、3、4、5、6、7、8，相對應乘上1987654321，再相加。
     * 相加後的值除以模數，也就是10，取餘數再以模數10減去餘數，若等於檢查碼，則驗證通過
     */
    $(document).on('blur', '.IdNumberIdentify', function (e) {
        var value = $(this).val();
        if (value == '') { return false; }
        var a = new Array('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'W', 'Z', 'I', 'O');
        var b = new Array(1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1);
        var c = new Array(2);
        var d;
        var e;
        var f;
        var g = 0;
        var h = /^[a-z](1|2)\d{8}$/i;
        //驗證填入身分證字號長度及格式
        if (value.length != 10) {
            MN.Alert('topCenter', 'error', '(身分證驗證)長度有誤，請重新輸入');
            $(this).val(null);
            return false;
        }
        if (value.search(h) == -1) {
            MN.Alert('topCenter', 'error', '(身分證驗證)格式錯誤，請重新輸入');
            $(this).val(null);
            return false;
        }
        else {
            d = value.charAt(0).toUpperCase();
            f = value.charAt(9);
        }
        if (/^[a-z]+$/.test(value.charAt(0))) {
            $(this).val($(this).val().toUpperCase());
            //alert('(身分證驗證)英文字母請用大寫');
            //$(this).val(null);
            //return false;
        }
        for (var i = 0; i < 26; i++) {
            if (d == a[i])//a==a
            {
                e = i + 10; //10
                c[0] = Math.floor(e / 10); //1
                c[1] = e - (c[0] * 10); //10-(1*10)
                break;
            }
        }
        for (var i = 0; i < b.length; i++) {
            if (i < 2) {
                g += c[i] * b[i];
            }
            else {
                g += parseInt(value.charAt(i - 1)) * b[i];
            }
        }
        if ((g % 10) == 0) {
            //MN.Alert("top", "success", "格式正確!");
            return true;
        }
        /*if ((10 - (g % 10)) != f)*/ if ((g % 10) != 0) {
            MN.Alert('topCenter', 'error', '(身分證驗證)格式錯誤');
            $(this).val(null);
            return false;
        }
        return true;
    });
    /* 項 目 計 算 方 法 說 明 
    統一編號 0 4 5 9 5 2 5 7 　 
    邏輯乘數 1 2 1 2 1 2 4 1 兩數上下對應相乘 
    乘    積 0 8 5 1 5 4 2 7
                   8     0   乘積直寫並上下相加
    --------------------------------------------                
    乘積之和 0 8 5 9 5 4 2 7 將相加之和再相加0+8+5+9+5+4+2+7=40最後結果,40能被10整除,故04595257符合邏輯。 
    *若第七位數字為 7 時
    統一編號 1 0 4 5 8 5 7 5 倒數號二位為 7 
    邏輯乘數 1 2 1 2 1 2 4 1 兩數上下對應相乘 
    乘    積 1 0 4 1 8 1 2 5
                   0   0 8   乘積直寫並上下相加
    ---------------------------------------------               
    乘積之和 1 0 4 1 8 1 1 5
                         0   再相加時最後第二位數取 0 或 1 均可。 
           1+0+4+1+8+1+1+5=21 　 
           1+0+4+1+8+1+0+5=20 　 
    最後結果中, 20 能被 10 整除, 故 10458575 符合邏輯。 
     */
    $(document).on('blur', '.CheckCompanyId', function (e) {
        var NO = $(this).val();
        if (NO == '') { return false; }
        var cx = new Array;
        cx[0] = 1;
        cx[1] = 2;
        cx[2] = 1;
        cx[3] = 2;
        cx[4] = 1;
        cx[5] = 2;
        cx[6] = 4;
        cx[7] = 1;
        var SUM = 0;
        //if (NO.length != 8) {
        //    alert('統編錯誤，要有 8 個數字');
        //    return;
        //}
        var cnum = NO.split("");
        if (cnum.length != 8) {
            MN.Alert('topCenter', 'error', '統編錯誤，要有 8 個 0-9 數字組合');
            $(this).val(null);
            return false;
        }

        for (i = 0; i <= 7; i++) {
            //if (NO.charCodeAt() < 48 || NO.charCodeAt() > 57) {
            //    alert('統編錯誤，要有 8 個 0-9 數字組合');
            //    return;
            //}
            SUM += cc(cnum[i] * cx[i]);
        }
        if (SUM % 10 == 0) {
            //alert('正確!');
        }
        else if (cnum[6] == 7 && (SUM + 1) % 10 == 0) {
            //alert('正確!');
        }
        else {
            MN.Alert('topCenter', 'error', '統編錯誤，請檢查統編正確性');
            $(this).val(null);
            return false;
        }
    }).on('keyup', '.CheckCompanyId', function (e) {
        var target = "[^0-9]"; //準備替代的文字, 可用 | 代表or
        var myRegExp = new RegExp(target, 'g'); //轉換成正規表示
        var replaceText = ""; //準備替換成的文字
        $(this).val($(this).val().replace(myRegExp, replaceText)); //開始替換
    });
    $(document).on('blur', '.number', function () { checkNum(this); })/*.on('keyup', '.number', function () { checkNum(this); })*/;
})

function timedCount() {
    if (comm_seconds == 0) {
        $(comm_obj).prop('disabled', false);
        if ($(comm_obj).prop('localName') == 'a' || $(comm_obj).prop('localName') == 'button')
            $(comm_obj).text(comm_text);
        else
            $(comm_obj).val(comm_text);
    }
    else {
        if ($(comm_obj).prop('localName') == 'a' || $(comm_obj).prop('localName') == 'button')
            $(comm_obj).text('請等待(' + comm_seconds + ')');
        else
            $(comm_obj).val('請等待(' + comm_seconds + ')');
        comm_seconds--;
        t = setTimeout(timedCount, 1000)
    }
}
function timedSet(obj, show_text, Seconds) {
    comm_obj = obj;
    comm_text = show_text;
    comm_seconds = Seconds || 5;
    timedCount();
}