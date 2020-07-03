import React from "react";
import axios from "axios";
import Swal from "sweetalert2";

export default class RecipesTable extends React.Component {
  getKeys = function () {
    if (this.props.data.length) return Object.keys(this.props.data[0]);
    else return [];
  };

  getHeader = function () {
    var keys = this.getKeys();
    if (keys.length) {
      return (
        <tr>
          <th>Name</th>
          <th>Category</th>
          <th></th>
        </tr>
      );
    } else {
      return null;
    }
  };

  getRowsData = function () {
    var items = this.props.data;
    var keys = this.getKeys();
    console.log(keys);
    //delete keys[2];
    return items.map((row, index) => {
      return (
        <RenderRow
          key={index}
          data={row}
          keys={keys}
          inventories={this.props.inventories}
          wishlists={this.props.wishlists}
          shoppinglists={this.props.shoppinglists}
        />
      );
    });
  };

  render() {
    return (
      <div>
        <table id="recipesList">
          <thead>{this.getHeader()}</thead>
          <tbody>{this.getRowsData()}</tbody>
        </table>
      </div>
    );
  }
}
const RenderRow = (props) => {
  async function addProductInventory(product_id, inventory_id) {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: formValues } = await Swal.fire({
      title: "Add Product",
      html:
        '<input id="swal-input1" placeholder="Quantity" class="swal2-input">' +
        '<input id="swal-input2" placeholder="Expire date (YYYY/MM/DD)" class="swal2-input">',
      focusConfirm: false,
      preConfirm: () => {
        let quantity = document.getElementById("swal-input1").value;
        let expire = document.getElementById("swal-input2").value;
        return [quantity, expire];
      },
    });

    if (formValues != null && formValues[0] != null && formValues[1] != null) {
      form.append("product", product_id);
      form.append("quantity", formValues[0]);
      form.append("expire", formValues[1]);
      form.append("uid", inventory_id);

      axios
        .post("http://localhost:1331/inventory/addproduct", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token },
          withCredentials: true,
        })
        .then((response) => {
          /* save this token inside localStorage */
          const token = response.headers["auth"];
          localStorage.setItem("auth", token);
        })
        .catch((error) => {
          console.log(error);
        });
    }
  }

  async function addProductWishlist(product_id, whishlist_id) {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: formValues } = await Swal.fire({
      title: "Add Product",
      html:
        '<input id="swal-input1" placeholder="Quantity" class="swal2-input">',
      focusConfirm: false,
      preConfirm: () => {
        let quantity = document.getElementById("swal-input1").value;
        return quantity;
      },
    });

    if (formValues != null) {
      form.append("product", product_id);
      form.append("quantity", formValues);
      form.append("uid", whishlist_id);

      axios
        .post("http://localhost:1331/wishlist/addproduct", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token },
          withCredentials: true,
        })
        .then((response) => {
          /* save this token inside localStorage */
          const token = response.headers["auth"];
          localStorage.setItem("auth", token);
        })
        .catch((error) => {
          console.log(error);
        });
    }
  }

  async function addProductShopping(product_id, shopping_id) {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: formValues } = await Swal.fire({
      title: "Add Product",
      html:
        '<input id="swal-input1" placeholder="Quantity" class="swal2-input">',
      focusConfirm: false,
      preConfirm: () => {
        let quantity = document.getElementById("swal-input1").value;
        return quantity;
      },
    });

    if (formValues != null) {
      form.append("product", product_id);
      form.append("quantity", formValues);
      form.append("uid", shopping_id);

      axios
        .post("http://localhost:1331/shopping/addproduct", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token },
          withCredentials: true,
        })
        .then((response) => {
          /* save this token inside localStorage */
          const token = response.headers["auth"];
          localStorage.setItem("auth", token);
        })
        .catch((error) => {
          console.log(error);
        });
    }
  }

  async function addProduct(e) {
    let lists = ["Inventory", "Wishlist", "Shoppinglist"];
    console.log(e);

    const { value: list } = await Swal.fire({
      title: "Select list",
      input: "select",
      inputOptions: lists,
      inputPlaceholder: "Select a list",
      showCancelButton: true,
      inputValidator: (value) => {
        return new Promise((resolve) => {
          if (value) {
            resolve();
          } else {
            resolve("You need to select one");
          }
        });
      },
    });

    const choice = lists[list];
    let names;

    switch (choice) {
      case "Inventory":
        names = props.inventories;
        break;
      case "Wishlist":
        names = props.wishlists;
        break;
      default:
        names = props.shoppinglists;
        break;
    }

    if (list) {
      const { value: index } = await Swal.fire({
        title: "Select " + choice,
        input: "select",
        inputOptions: Object.keys(names),
        inputPlaceholder: "Select a " + choice,
        showCancelButton: true,
        inputValidator: (value) => {
          return new Promise((resolve) => {
            if (value) {
              resolve();
            } else {
              resolve("You need to select one");
            }
          });
        },
      });

      const id = Object.values(names)[index];
      console.log(names);
      console.log(Object.values(names));
      console.log(id);
      switch (choice) {
        case "Inventory":
          addProductInventory(e, id);
          break;
        case "Wishlist":
          addProductWishlist(e, id);
          break;
        default:
          addProductShopping(e, id);
          break;
      }
    }
  }

  if (props.keys.length) {
    return (
      <tr>
        <td id="title">{props.data["_name"]}</td>
        <td id="title">{props.data["_category"]}</td>
        <td
          id="fav-button"
          onClick={() => {
            addProduct(props.data["_guid"]);
          }}
        >
          <span
            className="edit-button"
            role="img"
            aria-label="jsx-a11y/aria-proptypes"
            title="Add Product"
          >
            ➕
          </span>
        </td>
      </tr>
    );
  } else {
    return null;
  }
};
