let productTable = null;

$(function () {

    loadProducts();

});

function loadProducts() {

    $.get("/Product/GetList", function (data) {

        if (productTable != null)
            productTable.destroy();

        productTable = new Tabulator("#productGrid", {

            layout: "fitColumns",

            height: "700px",

            placeholder: "No products found.",

            data: data,

            columns: [

                {
                    title: "Brand Id",
                    field: "brandId",
                    width: 120
                },

                {
                    title: "Collection",
                    field: "collectionId",
                    width: 120
                },

                {
                    title: "Name",
                    field: "name"
                },

                {
                    title: "Source",
                    field: "sourceId",
                    width: 100
                },

                {
                    title: "Currency",
                    field: "currencyCode",
                    width: 100
                },

                {
                    title: "Active",
                    field: "isActive",
                    formatter: "tickCross",
                    hozAlign: "center",
                    width: 90
                }

            ]

        });

    });

}