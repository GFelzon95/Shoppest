var dataTable

$(document).ready(function(){
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#productTbl').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'name', width:'20%' },
            { data: 'price', width: '10%' },
            { data: 'quantity', width: '15%' },
            { data: 'productCategory.name', width: '20%' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group px-3" role="group">
                                <a asp-controller="Product" asp-action="Upsert" asp-route-id="@product.Id" class="btn btn-primary ms-2"><i class="bi bi-pencil-square"></i> Edit</a>
                                <a onClick="Delete('/admin/product/delete?id=@product.Id')" class="btn btn-danger ml-2"><i class="bi bi-trash-fill"></i> Delete</a>
                            </div>`;
                },
                width: '35%'
            }

        ]
    })
}


function Delete(url) { 
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}
