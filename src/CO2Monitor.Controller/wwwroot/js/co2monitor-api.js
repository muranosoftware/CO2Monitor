//////////////////////////////////////// ERRORS //////////////////////////////////////////////////

function showErrorWindow(error) {
    var msg = error.message;
    
    if ("response" in error && "data" in error.response) {
        data = error.response.data;
        if (typeof data === "object")
            Object.keys(data).forEach(p => msg += "\n" + p + ": " + data[p]);
        else
            msg += "\n" + data;
    }

    window.alert(msg);
}

//////////////////////////////////////// DEVICES //////////////////////////////////////////////////
function getDevices(thenFunc) {
    axios.get("/api/devices", {
        params: null
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}


function deleteDevice(device, thenFunc) {
    axios.delete("/api/devices", {
        data : device
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}


/////////////////////////////////////// REMOTE ///////////////////////////////////////////////////

function getRemote(thenFunc) {
    axios.get("/api/devices/remote", {
        params: null
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}


function createRemote(name, address, fields, actions, thenFunc) {
    axios.post("/api/devices/remote", {
        name: name,
        address: address,
        info: {
            actions: actions,
            fields: fields,
            events: []
        }
    }).then(function (response) {
            if (thenFunc) {
                thenFunc(response.data);
            }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

//////////////////////////////////////////////// TIMERS ///////////////////////////////////////////////////

function getTimers(thenFunc) {
    axios.get("/api/devices/timers", {
        params: null
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

function createTimer(name, time, thenFunc) {
    axios.post("/api/devices/timers", {
        name: name,
        alarmTime: time
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

function patchTimer(id, name, time, thenFunc) {
    axios.patch("/api/devices/timers", {
        id: id,
        name: name,
        alarmTime: time
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

////////////////////////////////////////////////// RULES ///////////////////////////////////////////////


function getRules(thenFunc) {
    axios.get("/api/rules", {
        params: null
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

function createRule(rule, thenFunc) {
    axios.post("/api/rules", rule, {
        params: null
    }).then(function (response) {
            $("#newRuleModal").modal("hide");
            fillRuleTable();
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

function editRule(rule, thenFunc) {
    axios.patch("/api/rules", rule, {
        params: null
    }).then(function (response) {
        $("#newRuleModal").modal("hide");
        fillRuleTable();
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

function deleteRule(id, thenFunc) {
    axios.delete("/api/rules", {
        params: {
            id: id
        }
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

////////////////////////////////////////////////// EXTENTIONS ///////////////////////////////////////////

function getExtensionTypes(thenFunc) {
    axios.get("/api/devices/extensionTypes", {
        params: null
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}


function createExtension(deviceId, type, parameter, thenFunc) {
    axios.post("/api/devices/extensions", {
        type: type,
        parameter: parameter
    }, {
        params: {
            deviceId: deviceId,
        }
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}

////////////////////////////////////////////////// LOGS ////////////////////////////////////////////////

function getLog(thenFunc) {
    axios.get("/api/log", {
        params: null
    }).then(function (response) {
        if (thenFunc) {
            thenFunc(response.data);
        }
    }).catch(function (error) {
        showErrorWindow(error);
    });
}
