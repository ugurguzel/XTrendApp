let scanJobTable = null;

$(function () {

    loadScanJobs();

});

function loadScanJobs() {

    $.get("/ScanJob/GetList", function (data) {

        if (scanJobTable != null) {

            scanJobTable.destroy();

        }

        scanJobTable = new Tabulator("#scanJobGrid", {

            layout: "fitColumns",

            height: "600px",

            placeholder: "No scan jobs found.",

            data: data,

            columns: [

                {
                    title: "Job",
                    field: "name"
                },

                {
                    title: "Source",
                    field: "source"
                },

                {
                    title: "Products",
                    field: "productLimit",
                    hozAlign: "center",

                    editor: function (cell, onRendered, success, cancel) {

                        const input = document.createElement("input");

                        input.type = "number";
                        input.min = "1";
                        input.max = "100";
                        input.step = "1";

                        input.value = cell.getValue() ?? 25;

                        input.style.width = "100%";
                        input.style.boxSizing = "border-box";

                        let saving = false;

                        onRendered(function () {

                            input.focus();
                            input.select();

                        });

                        function saveValue() {

                            if (saving)
                                return;

                            let value = parseInt(input.value);

                            if (isNaN(value))
                                value = 1;

                            value = Math.max(1, Math.min(100, value));

                            input.value = value;

                            saving = true;

                            const rowData = cell.getRow().getData();

                            $.ajax({

                                url: "/ScanJob/UpdateProductLimit",

                                type: "POST",

                                data: {
                                    id: rowData.id,
                                    productLimit: value
                                },

                                success: function (response) {

                                    if (response.success) {

                                        success(value);

                                    }
                                    else {

                                        Swal.fire({
                                            icon: "error",
                                            title: "Error",
                                            text: response.message
                                        });

                                        cancel();
                                    }

                                },

                                error: function () {

                                    Swal.fire({
                                        icon: "error",
                                        title: "Error",
                                        text: "Product limit could not be updated."
                                    });

                                    cancel();

                                },

                                complete: function () {

                                    saving = false;

                                }

                            });

                        }

                        input.addEventListener("keydown", function (e) {

                            if (e.key === "Enter") {

                                e.preventDefault();

                                saveValue();

                            }

                            if (e.key === "Escape") {

                                e.preventDefault();

                                cancel();

                            }

                        });

                        input.addEventListener("blur", function () {

                            saveValue();

                        });

                        input.addEventListener("input", function () {

                            let value = parseInt(input.value);

                            if (!isNaN(value) && value > 100)
                                input.value = 100;

                            if (!isNaN(value) && value < 1)
                                input.value = 1;

                        });

                        return input;
                    }
                },

                {
                    title: "Enabled",
                    field: "isEnabled",
                    hozAlign: "center",
                    formatter: "tickCross"
                },

                {
                    title: "Last Run",
                    field: "lastRun",
                    formatter: function (cell) {

                        return cell.getValue() ?? "-";

                    }
                },

                {
                    title: "",
                    hozAlign: "center",
                    formatter: function () {

                        return "<button class='btn btn-primary btn-sm'>Run</button>";

                    },

                    cellClick: function (e, cell) {

                        runJob(cell.getRow().getData().code);

                    }

                }

            ]

        });

    });

}

function runJob(code) {

    Swal.fire({

        title: "Run Scan Job?",
        text: "The selected scan job will be started.",
        icon: "question",

        showCancelButton: true,

        confirmButtonText: "Run",

        cancelButtonText: "Cancel"

    }).then((result) => {

        if (!result.isConfirmed)
            return;

        $.ajax({

            url: "/ScanJob/Run",

            type: "POST",

            data: {
                code: code
            },

            success: function (response) {

                if (response.success) {

                    Swal.fire({

                        icon: "success",

                        title: "Completed",

                        text: response.message

                    });

                    loadScanJobs();

                }
                else {

                    Swal.fire({

                        icon: "error",

                        title: "Error",

                        text: response.message

                    });

                }

            },

            error: function () {

                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text: "Unexpected error."

                });

            }

        });

    });

}