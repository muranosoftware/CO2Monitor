
function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}

function serializedArrToObj(arr) {
    var obj = {};
    $.map(arr, function (n, i) {
        obj[n['name']] = n['value'];
    });
    return obj;
}

function createTableRow(data, columns) {
    var tr = document.createElement('tr');
    jQuery.data(tr, "data", data);
    for (let i in columns) {
        c = columns[i];
        td = document.createElement('td');
        td.innerHTML = data[c];
        tr.appendChild(td);
    }
    return tr;
}

function checkTimeFormat(time) {
    reg = /^\d\d[:]\d\d[:]\d\d$/i;
    if (!reg.test(time)) {
        window.alert("Invalid time format [" + time + "] Must be hh:mm:ss");
        return false;
    }

    reg = /\d\d/g;
    hms = [...time.match(reg)];
    if (hms[0] > "23") {
        window.alert("Hours must be less 24");
        return false;
    }

    if (hms[1] > "59") {
        window.alert("Minutes must be less 60");
        return false;
    }

    if (hms[2] > "59") {
        window.alert("Seconds must be less 60");
        return false;
    }

    return true;
}

function fillTable(tableId, data, columns) {

    var tbody = document.createElement('tbody');
    for (let i in data) {
        tbody.appendChild(createTableRow(data[i], columns));
    }
    var table = document.getElementById(tableId);
    table.replaceChild(tbody, table.getElementsByTagName("tbody")[0]);
}


function createTr(params) {
    var tr = document.createElement('tr');
    for (var i = 0; i < arguments.length; i++) {
        var td = document.createElement('td');
        td.appendChild(arguments[i]);
        tr.appendChild(td);
    }
    return tr;
}


function createButton(text, onclick, cls = null) {
    var b = document.createElement('button');
    b.onclick = onclick;
    b.innerHTML = text;
    if (cls)
        b.setAttribute("class", cls);
    var div = document.createElement("div");
    div.setAttribute("align", "center");
    div.appendChild(b);

    return div;
}

function createSelect(optionValueData, selector, onchange = null, cls = null) {
    var slct = document.createElement('select');
    for (var i = 0; i < optionValueData.length; i++) {
        var opt = document.createElement('option');
        opt.value = optionValueData[i];
        jQuery.data(opt, "data", optionValueData[i]);
        opt.innerHTML = selector(optionValueData[i]);
        slct.appendChild(opt);
    }

    if (cls)
        slct.setAttribute("class", cls);

    if (onchange)
        slct.onchange = onchange;
    return slct;
}

function createInput(cls = null, disabled = false) {
    var input = document.createElement("input");
    input.disabled = disabled;
    if (cls)
        input.setAttribute("class", cls);
    return input;
}
