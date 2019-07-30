function deleteDeviceClicked(event) {
    var device = jQuery.data(event.srcElement.closest("tr"), "data");

    if (window.confirm(`Do you really want to delete device [${device.name}:${device.id}]?`)) {
        deleteDevice(device, () => {
            fillTimerTable();
            fillRemoteTable();
        });
    }
}


function fillRemoteTable() {
    getRemote((remote) => {
        for (let i in remote) {
            r = remote[i];
            r.del = '<div align="center"><button class="btn btn-danger" onclick="deleteDeviceClicked(event)">Del</button></div>';
            if (r.status === "Ok") {
                r.statusIcon = '<div align="center"><img src="img/ok-64.png" alt="NotAccessible" width="32"></div>';
            } else {
                r.statusIcon = '<div align="center"><img src="img/not-accessible.png" alt="NotAccessible" width="32"></div>';
            }
        }
        fillTable("remoteTable", remote, ["name", "id", "statusIcon", "state", "address", "del"]);
    });
}

function showEditTimerModal(event) {
    var timer = jQuery.data(event.srcElement.closest("tr"), "data");
    $("#editTimerForm").data("data", timer);
    document.getElementById("inputTimerEditedName").value = timer.name;
    document.getElementById("inputTimerEditedTime").value = timer.alarmTime;
    $('#editTimerModal').modal('show');
}

function fillTimerTable() {
    getTimers((timers) => {
        for (let i in timers) {
            t = timers[i];
            t.del = `<div align="center"><button class="btn btn-danger" onclick="deleteDeviceClicked(event)">Del</button></div>`;
            t.edit = `<div align="center"><button class="btn btn-info" onclick="showEditTimerModal(event)">Edit</button></div>`;
        }
        fillTable("timerTable", timers, ["name", "id", "alarmTime", "edit", "del"]);
    });
}


function newRuleModalButtonClicked(event) {
    var form = document.getElementById("newRuleForm");
    jQuery.data(form, "command", "new");
    jQuery.data(form, "rule", {});

    document.getElementById("createNewRuleButton").innerHTML = "Create";

    $('#newRuleModal').modal('show');
}

function showEditRuleModal(event) {
    var rule = jQuery.data(event.srcElement.closest("tr"), "data");

    var form = document.getElementById("newRuleForm");
    jQuery.data(form, "command", "edit");
    jQuery.data(form, "rule", rule);

    document.getElementById("createNewRuleButton").innerHTML = "Save";


    $('#newRuleModal').modal('show');
}

function deleteRuleClicked(event) {
    var rule = jQuery.data(event.srcElement.closest("tr"), "data");
    if (window.confirm(`Do you really want to delete rule [${rule.name}:${rule.id}]?`)) {
        deleteRule(rule.id, () => {
            fillRuleTable();
        });
    }
}

function fillRuleTable() {
    getRules((rules) => {
        for (let i in rules) {
            r = rules[i];
            r.desc = `Src: { Id = ${r.sourceDeviceId}, Event = "${r.event.name}}", Target: { Id = ${r.targetDeviceId}, Action="${r.action.path}", ArgumentSource="${r.argumentSource}" , ConstantArgument="${r.actionArgument}"}, Conditons="${JSON.stringify(r.conditions.map(c => c.deviceId + "." + c.field.name + " " + c.conditionType + " " + c.conditionArgument))}"`;
            r.edit = '<button class="btn btn-info" onclick="showEditRuleModal(event)">Edit</button>';
            r.del = '<button class="btn btn-danger" onclick="deleteRuleClicked(event)">Del</button>';
        }
        fillTable("ruleTable", rules, ["name", "id", "desc", "edit", "del"]);
    });
}

function fillLogTable() {
    getLog((records) => {
        var prev = 0;
        var tableRecords = [];
        for (let i in records) {
            r = records[i];
            t = new Date(r.time);
            r.formatTime = t.toLocaleTimeString("ru-RU");
            if (r.logLevel === "Error") {
                r.level = '<b>Error</b>';
            } else {
                r.level = r.logLevel.substring(0, 4);
            }

            var cur = 3600 * t.getHours() + 60 * t.getMinutes() + t.getSeconds();
            if (cur > prev) {
                tableRecords.push({ formatTime: `<b> ${t.toISOString().substring(0, 10).replace(/-/g, '.')} </b>`, level: "", message: "" });
            }
            prev = cur;

            tableRecords.push(r);
        }
        fillTable("logTable", tableRecords, ["formatTime", "level", "message"]);
    });
}

$("#newTimerModal").on("shown.bs.modal", () => {
    getDevices((devices) => {
        var id = 1;
        if (devices.length > 0) {
            id = Math.max.apply(Math, devices.map(d => d.id)) + 1;
        }

        inputTimerName = document.getElementById("inputTimerName");
        inputTimerName.value = "Timer" + id;
    });
});

var newTimerBtn = document.getElementById("createNewTimerButton");
newTimerBtn.onclick = function () {
    timerData = serializedArrToObj($("#newTimerForm").serializeArray());
    if (!checkTimeFormat(timerData.time))
        return;

    if (isEmptyOrSpaces(timerData.name)) {
        window.alert("Timer Name must be not empty!");
        return;
    }

    createTimer(timerData.name, timerData.time, () => {
        $('#newTimerModal').modal('hide');
        fillTimerTable();
    });
};

var saveTimerChanchedBtn = document.getElementById("saveTimerChanchedButton");
saveTimerChanchedBtn.onclick = function () {

    timer = $("#editTimerForm").data("data");

    timerData = serializedArrToObj($("#editTimerForm").serializeArray());
    if (!checkTimeFormat(timerData.time))
        return;

    if (isEmptyOrSpaces(timerData.name)) {
        window.alert("Timer Name must be not empty!");
        return;
    }

    patchTimer(timer.id, timerData.name, timerData.time, () => {
        $('#editTimerModal').modal('hide');
        fillTimerTable();
    });
};

function deleteRowFromTableClick(event) {
    var body = event.srcElement.closest("tbody");
    var row = event.srcElement.closest("tr");
    body.removeChild(row);
}


function enableEnumValuesInput(event) {
    var typeSelect = event.srcElement;
    
    var input = typeSelect.closest("tr").getElementsByTagName("input")[1];
    if (typeSelect.value === "Enum") {
        input.disabled = false;
    } else {
        input.disabled = true;
        input.value = null;
    }
}

var addActionBtn = document.getElementById("addActionNewRemoteButton");
addActionBtn.onclick = function () {
    var table = document.getElementById("newRemoteActionTable");
    var tbody = table.getElementsByTagName("tbody")[0];

    var tr = createTr(
        createInput("form-control"),
        createSelect(["Void", "Float", "String", "Enum"], x => x, enableEnumValuesInput, "form-control"),
        createInput("form-control", true),
        createButton("Del", deleteRowFromTableClick, "btn btn-danger"));
    tbody.appendChild(tr);
};

var addFieldBtn = document.getElementById("addFieldNewRemoteButton");
addFieldBtn.onclick = function () {
    var table = document.getElementById("newRemoteFieldTable");
    var tbody = table.getElementsByTagName("tbody")[0];

    var tr = createTr(
        createInput("form-control"),
        createSelect(["Float", "Time", "String", "Enum"], (x) => x, enableEnumValuesInput, "form-control"),
        createInput("form-control", true),
        createButton("Del", deleteRowFromTableClick, "btn btn-danger"));


    tbody.appendChild(tr);
};


function selectArgumentSourceOnChange() {
    slct = document.getElementById("selectArgumentSource");
    if (slct.value === "Constant") {
        document.getElementById("inputValue").disabled = false;
    } else {
        document.getElementById("inputValue").disabled = true;
    }
}

var addExtensionNewRemoteBtn = document.getElementById("addExtensionNewRemoteButton");
addExtensionNewRemoteBtn.onclick = function () {
    getExtensionTypes(types => {
        var tbody = document.getElementById("newRemoteExtensionTableBody");
        tbody.appendChild(
            createTr(
                createSelect(types, (x) => x, null, "form-control"),
                createInput("form-control"),
                createButton("Del", deleteRowFromTableClick, "btn btn-danger")
            )
        );
    });
};

var createRemoteBtn = document.getElementById("createNewRemoteButton");
createRemoteBtn.onclick = function () {
    var info = serializedArrToObj($("#newRemoteForm").serializeArray());
    console.debug(info);
    var actions = [];
    var fields = [];
    var extensions = [];

    $("#newRemoteActionTableBody").find("tr").each(function (i, row) {
        var tds = row.getElementsByTagName("td");

        var type = tds[1].getElementsByTagName("select")[0].value;
        var vals = null;
        if (type === "Enum") {
            vals = tds[2].getElementsByTagName("input")[0].value.split(",").map(x => x.trim());
        }

        actions.push({
            path: tds[0].getElementsByTagName("input")[0].value.trim(),
            argument: {
                type: type,
                enumValues: vals
            }
        });
    });

    $("#newRemoteFieldTableBody").find("tr").each(function (i, row) {
        var tds = row.getElementsByTagName("td");

        var type = tds[1].getElementsByTagName("select")[0].value;
        var vals = null;
        if (type === "Enum") {
            vals = tds[2].getElementsByTagName("input")[0].value.split(",").map(x => x.trim());
        }

        fields.push({
            name: tds[0].getElementsByTagName("input")[0].value.trim(),
            type: {
                type: type,
                enumValues: vals
            }
        });
    });

    $("#newRemoteExtensionTableBody").find("tr").each(function (i, row) {
        var tds = row.getElementsByTagName("td");

        extensions.push({
            type: tds[0].getElementsByTagName("select")[0].value,
            parameter: tds[1].getElementsByTagName("input")[0].value
        });
    });

    info.name = info.name.trim();
    info.address = info.address.trim();
    while (info.address[info.address - 1] === "/")
        info.address = info.address.slice(0, -1);

    createRemote(info.name, info.address, fields, actions, (remote) => {

        for (let i in extensions) {
            createExtension(remote.id, extensions[i].type, extensions[i].parameter, () => { });
        }
        fillRemoteTable();

        $('#newRemoteModal').modal('hide');
    });
};

function ruleConditionDeviceSelectOnChange(event) {
    var deviceSelect = event.srcElement;
    var device = jQuery.data(deviceSelect.options[deviceSelect.selectedIndex], "data");

    var tr = deviceSelect.closest("tr");
    var oldFieldSelect = tr.getElementsByTagName("select")[1];
    var td = tr.getElementsByTagName("select")[1].closest("td");
    td.removeChild(oldFieldSelect);

    td.appendChild(createSelect(device.info.fields, x => x.name, null, "form-control"));
}


$("#newRuleModal").on("shown.bs.modal", () => {
    getRules((rules) => {
        var id = 1;
        if (rules.length > 0) {
            id = Math.max.apply(Math, rules.map(d => d.id)) + 1;
        }
        inputRuleName = document.getElementById("inputRuleName");
        inputRuleName.value = "Rule" + id;
    });


    getDevices((devices) => {

        var form = document.getElementById("newRuleForm");
        var command = jQuery.data(form, "command");
        var rule = jQuery.data(form, "rule");

        var selectTargetDeviceId = document.getElementById("selectTargetDeviceId");
        var selectAction = document.getElementById("selectAction");

        $("#selectSourceDeviceId").empty();
        $("#selectTargetDeviceId").empty();
        document.getElementById("newRuleConditionTableBody").innerHTML = "";

        var selectSourceDeviceId = document.getElementById("selectSourceDeviceId");
        var selectEvent = document.getElementById("selectEvent");

        selectSourceDeviceId.onchange = function () {
            $("#selectEvent").empty();
            events = devices.find(d => d.id == selectSourceDeviceId.value).info.events;

            for (let i in events) {
                var e = events[i];
                var opt = document.createElement("option");
                opt.innerHTML = e.name;
                opt.value = e.name;
                jQuery.data(opt, "data", e);
                selectEvent.appendChild(opt);
            }
        };

        selectTargetDeviceId.onchange = function () {
            $("#selectAction").empty();
            actions = devices.find(d => d.id == selectTargetDeviceId.value).info.actions;

            for (let i in actions) {
                var a = actions[i];
                var opt = document.createElement("option");
                opt.innerHTML = a.path;
                opt.value = a.path;
                jQuery.data(opt, "data", a);
                selectAction.appendChild(opt);
            }
        };

        for (let i in devices) {
            let d = devices[i];
            if (d.info.events.length > 0) {
                var opt = document.createElement("option");
                opt.innerHTML = d.name + ":" + d.id;
                opt.value = d.id;
                selectSourceDeviceId.appendChild(opt);
            }

            if (d.info.actions.length > 0) {
                var opt = document.createElement("option");
                opt.innerHTML = d.name + ":" + d.id;
                opt.value = d.id;
                selectTargetDeviceId.appendChild(opt);
            }
        }

        createNewRuleButton.onclick = function () {
            var conditions = [];

            $("#newRuleConditionTableBody").find("tr").each(function (i, row) {
                var tds = row.getElementsByTagName("td");

                var deviceSelect = tds[0].getElementsByTagName("select")[0];
                var device = jQuery.data(deviceSelect.options[deviceSelect.selectedIndex], "data");
                var fieldSelect = tds[1].getElementsByTagName("select")[0];
                var field = jQuery.data(fieldSelect.options[fieldSelect.selectedIndex], "data");
                var condTypeSelect = tds[2].getElementsByTagName("select")[0];
                condType = jQuery.data(condTypeSelect.options[condTypeSelect.selectedIndex], "data").cond;

                conditions.push({
                    deviceId: device.id,
                    field: field,
                    conditionType: condType,
                    conditionArgument: tds[3].getElementsByTagName("input")[0].value
                });
            });

            rule.name = document.getElementById("inputRuleName").value;
            rule.sourceDeviceId = selectSourceDeviceId.value;
            rule.event = jQuery.data(selectEvent.options[selectEvent.selectedIndex], "data");
            rule.targetDeviceId = selectTargetDeviceId.value;
            rule.action = jQuery.data(selectAction.options[selectAction.selectedIndex], "data");
            rule.actionArgument = document.getElementById("inputValue").value;
            rule.argumentSource = document.getElementById("selectArgumentSource").value;
            rule.conditions = conditions;

            if (command == "new") {
                createRule(rule, () => {
                    $("#newRuleModal").modal("hide");
                    fillRuleTable();
                });
            } else {
                editRule(rule, () => {
                    $("#newRuleModal").modal("hide");
                    fillRuleTable();
                });
            }
        };


        addRuleConditionBtn = document.getElementById("addRuleConditionButton");
        addRuleConditionBtn.onclick = () => {
            var table = document.getElementById("newRuleConditionTable");
            var tbody = table.getElementsByTagName("tbody")[0];
            var signs = [{ cond: "Equal", sign: "==" },
            { cond: "NotEqual", sign: "!=" },
            { cond: "GreaterThan", sign: ">" },
            { cond: "LessTahn", sign: "<" },
            { cond: "GreaterThanOrEqual", sign: ">=" },
            { cond: "LessThanOrEqual", sign: "<=" }];
            var deviceSelect = createSelect(devices, x => x.name + "." + x.id, ruleConditionDeviceSelectOnChange, "form-control");

            tr = createTr(
                createSelect(devices, x => x.name + "." + x.id, ruleConditionDeviceSelectOnChange, "form-control"),
                createSelect(devices[0].info.fields, x => x.name, null, "form-control"),
                createSelect(signs, x => x.sign, null, "form-control"),
                createInput("form-control"),
                createButton("Del", deleteRowFromTableClick, "btn btn-danger"));
            tbody.appendChild(tr);

        };


        selectSourceDeviceId.onchange();
        selectTargetDeviceId.onchange();

        if (rule.name)
            inputRuleName.value = rule.name;

        if (rule.sourceDeviceId) {
            selectSourceDeviceId.value = rule.sourceDeviceId;
            selectSourceDeviceId.onchange();
        }

        if (rule.event)
            selectEvent.value = rule.event.name;

        if (rule.targetDeviceId) {
            selectTargetDeviceId.value = rule.targetDeviceId;
            selectTargetDeviceId.onchange();
        }

        if (rule.action)
            selectAction.value = rule.action.path;

        if (rule.actionArgument)
            document.getElementById("inputValue").value = rule.actionArgument;

        if (rule.argumentSource) {
            document.getElementById("selectArgumentSource").value = rule.argumentSource;
            selectArgumentSourceOnChange();
        }
        if (rule.conditions) {
            for (let i in rule.conditions)
                addRuleConditionBtn.onclick();

            $("#newRuleConditionTableBody").find("tr").each(function (i, row) {
                var tds = row.getElementsByTagName("td");

                cond = rule.conditions[i];

                var deviceSelect = tds[0].getElementsByTagName("select")[0];

                for (let j = 0; j < deviceSelect.options.length; j++) {
                    var opt = deviceSelect.options[j];
                    if (cond.deviceId == jQuery.data(opt, "data").id) {
                        opt.selected = true;
                        ruleConditionDeviceSelectOnChange({ srcElement: deviceSelect });
                        break;
                    }
                }

                var fieldSelect = tds[1].getElementsByTagName("select")[0];

                for (let j = 0; j < fieldSelect.options.length; j++) {
                    var opt = fieldSelect.options[j];
                    if (cond.field.name == jQuery.data(opt, "data").name) {
                        opt.selected = true;
                        break;
                    }
                }

                var condTypeSelect = tds[2].getElementsByTagName("select")[0];

                for (let j = 0; j < condTypeSelect.options.length; j++) {
                    var opt = condTypeSelect.options[j];
                    if (cond.conditionType == jQuery.data(opt, "data").cond) {
                        opt.selected = true;
                        break;
                    }
                }

                tds[3].getElementsByTagName("input")[0].value = cond.conditionArgument;

            });

        }

    });
});

function updateTables() {
    fillTimerTable();
    fillRemoteTable();
    fillRuleTable();
    fillLogTable();
}

updateTables();

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/events")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ServerEvent", function (message) {
    console.log(`ServerEvent: ${message}`);
    updateTables();
});



async function start() {
    try {
        await connection.start();
        console.log("connected");
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
}

connection.onclose(async () => {
    await start();
});

connection.start().then(() => {
    console.log("connected");
});


