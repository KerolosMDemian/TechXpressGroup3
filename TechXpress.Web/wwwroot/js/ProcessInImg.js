function Delete(url) {
    swal({
        title: "Click Ok if you want to delete the item",
        text: "If click ok it will be deleted permanently",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(reloadPage, 3000);
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}

function reloadPage() {
    window.location.reload();
}

function triggerImageChange(oldImageUrl) {
    // Trigger the file input to select a new image
    const fileInput = document.getElementById('newImageInput');
    fileInput.click();

    fileInput.onchange = function () {
        const file = fileInput.files[0];
        if (file) {
            swal({
                title: "Do you want to update the image?",
                text: "If you click OK, the previous image will be permanently deleted and replaced with the new image.",
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((result) => {
                if (result) {
                    // Create a FormData object to send the file to the server
                    const formData = new FormData();
                    formData.append('newImage', file); // Append the selected file
                    formData.append('Id', oldImageUrl); // Append the old image URL (ID)

                    // AJAX request to update the image on the server
                    $.ajax({
                        url: '/Product/ChangeAPic',
                        type: 'POST',
                        data: formData,
                        processData: false,  // Don't process the data
                        contentType: false,  // Don't set content type, jQuery will automatically handle it
                        success: function (response) {
                            if (response.success) {
                                // Show success message and reload page after a short delay
                                swal({
                                    title: 'Success!',
                                    text: response.message,
                                    icon: 'success',
                                    button: { text: 'Ok', className: 'btn btn-secondary' }
                                }).then(() => {
                                    window.location.reload();  // Reload page to reflect changes
                                });
                            } else {
                                // Show error message if update fails
                                swal({
                                    title: 'Failed!',
                                    text: response.message,
                                    icon: 'error',
                                    button: { text: 'Ok', className: 'btn btn-secondary' }
                                });
                            }
                        },
                        error: function () {
                            // Handle error in case AJAX fails
                            swal({
                                title: 'Error!',
                                text: "An error occurred while deleting the order.",
                                icon: 'error',
                                button: { text: 'Ok', className: 'btn btn-secondary' }
                            });
                        }
                    });
                }
            });
        }
    };
}
