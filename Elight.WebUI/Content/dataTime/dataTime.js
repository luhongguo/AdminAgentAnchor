///timeFormat='data'只获取日期 time 获取当前日期时间， day 当前天数的前几天
function getNowFormatDate(timeFormat, day) {
    var date = new Date();
    date.setDate(date.getDate() + day);
    var seperator1 = "-";
    var seperator2 = ":";
    var year = date.getFullYear();//获取年份
    var month = date.getMonth() + 1;//获取月份
    var strDate = date.getDate();//获取日期
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = "";
    if (timeFormat == "data") {
        currentdate = year + seperator1 + month + seperator1 + strDate
    } else if (timeFormat = "time") {
        currentdate = year + seperator1 + month + seperator1 + strDate
            + " " + date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
    }
    return currentdate;
}