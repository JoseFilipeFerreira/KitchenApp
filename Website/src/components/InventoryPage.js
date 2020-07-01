import React from "react";
import Swal from "sweetalert2";
import axios from "axios";
import RecipesTable from "./RecipesTable";

export default class InventoryList extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      recipes: [],
    };
  }

  addProductMenu = () => {
    let token = localStorage.getItem("auth");
    axios
      .get(
        "http://localhost:1331/product/getall",
        {
          headers: { auth: token },
        },
        { withCredentials: true }
      )
      .then(async (response) => {
        console.log(response.data);
        let json = response.data;
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
        var r = [];
        let i = 0;
        var p = "";
        for (p in json) {
          r[i++] = json[p]["_category"];
        }
        var categories = Array.from(new Set(r));
        let sc = [];
        for (i = 0; i < categories.length; i++) {
          sc[i] = categories[i];
        }
        this.setState({
          categories: sc,
        });

        this.showCategories(json, this.state.categories);
      })
      .catch((error) => {
        console.log(error);
      });
  };

  addProduct = async (product) => {
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
      form.append("product", product["_guid"]);
      form.append("quantity", formValues[0]);
      form.append("expire", formValues[1]);
      form.append("uid", this.props.inventory_id);

      axios
        .post("http://localhost:1331/inventory/addproduct", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token },
          withCredentials: true,
        })
        .then((response) => {
          /* save this token inside localStorage */
          const token = response.headers["auth"];
          localStorage.setItem("auth", token);
          window.location.reload();
        })
        .catch((error) => {
          console.log(error);
        });
    }
  };

  showCategories = async (json, categories) => {
    const { value: category } = await Swal.fire({
      title: "Select category",
      input: "select",
      inputOptions: categories,
      inputPlaceholder: "Select a category",
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

    if (category) {
      this.showProducts(json, categories[category]);
    }
  };

  showProducts = async (json, category) => {
    var products = [];
    var ids = [];
    let i = 0;
    var p = "";
    for (p in json) {
      if (json[p]["_category"] === category) products[i] = json[p]["_name"];
      ids[i++] = json[p]["_guid"];
    }
    await Swal.fire({
      title: "Select product",
      input: "select",
      inputOptions: products,
      inputPlaceholder: "Select a category",
      showCancelButton: true,
      cancelButtonText: "Back",
      inputValidator: (value) => {
        return new Promise((resolve) => {
          if (value) {
            resolve();
          } else {
            resolve("You need to select one");
          }
        });
      },
    }).then((result) => {
      if (!result.value) {
        this.showCategories(json, this.state.categories);
      } else {
        this.addProduct(json[result.value]);
      }
    });
  };

  removeProduct = async (uid) => {
    let token = localStorage.getItem("auth");
    Swal.fire({
      title: "Remove Product",
      text: "Do you want to remove this product?",
      icon: "question",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, remove it!",
    }).then((result) => {
      if (result.value) {
        let form = new FormData();
        let token = localStorage.getItem("auth");
        form.append("uid", this.props.inventory_id);
        form.append("prod", uid);

        axios
          .post("http://localhost:1331/inventory/removeproduct", form, {
            headers: { "Content-Type": "multipart/form-data", auth: token },
            withCredentials: true,
          })
          .then((response) => {
            /* save this token inside localStorage */
            const token = response.headers["auth"];
            localStorage.setItem("auth", token);
            window.location.reload();
          })
          .catch((error) => {
            console.log(error);
          });
      }
    });
  };

  editProduct = async (uid) => {};

  showItems = () => {
    let json = this.props.items;
    json.sort(function (a, b) {
      return a._name.localeCompare(b._name);
    });
    return json.map((item, index) => {
      console.log(item);
      return (
        <tr>
          <td>{item._name}</td>
          <td>{item._stock}</td>
          <td>{item._consume_before.substring(0, 10)}</td>
          <td className="table-edit" key={"edit" + item._guid}>
            <span
              onClick={() => {
                this.editProduct(item._guid);
              }}
            >
              ✏️
            </span>
          </td>
          <td className="table-edit" key={"remove" + item._guid}>
            <span
              onClick={() => {
                this.removeProduct(item._guid);
              }}
            >
              ❌
            </span>
          </td>
        </tr>
      );
    });
  };

  getInventoryRecipes = () => {
    if (this.props.items.length) {
      let token = localStorage.getItem("auth");
      const form = new FormData();
      form.append("inventory", this.props.inventory_id);
      axios
        .post("http://localhost:1331/recipe/search", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token },
          withCredentials: true,
        })
        .then((response) => {
          this.setState({ recipes: response.data });
          console.log(response.data);
        })
        .catch((error) => {
          console.log(error);
        });
    } else {
      Swal.fire(
        'Nope!',
        "You don't have items on this inventory",
        'error'
      )
    }
    
  };

  showButtons = () => {
    if (this.props.shared) {
      return (
        <div className="inventory-button">
          <input
            className="create-button"
            type="button"
            value="Add product"
            onClick={this.addProductMenu}
          ></input>
          <input
            className="create-button"
            type="button"
            value="Show recipes"
            onClick={this.getInventoryRecipes}
          ></input>
        </div>
      );
    } else {
      return (
        <div className="inventory-button">
          <input
            className="create-button"
            type="button"
            value="Add product"
            onClick={this.addProductMenu}
          ></input>
          <input
            className="create-button"
            type="button"
            value="Edit inventory"
            onClick={this.editInventory}
          ></input>
          <input
            className="create-button"
            type="button"
            value="Share inventory"
            onClick={this.shareInventory}
          ></input>
          <input
            className="create-button"
            type="button"
            value="Show recipes"
            onClick={this.getInventoryRecipes}
          ></input>
        </div>
      );
    }
  };

  showRecipes = () => {
    if (this.state.recipes.length) {
      return (
        <article className="inventories">
          <RecipesTable data={this.state.recipes} stared={false}/>
        </article>
      );
    }
  };

  render() {
    return (
      <section className="grid">
        <article className="inventories">
          <div className="inventories-text">{this.props.inventory_name}</div>
          <table id="inventoryList">
            <thead>
              <tr>
                <th>Name</th>
                <th>Quantity</th>
                <th>Expire</th>
                <th></th>
                <th></th>
              </tr>
            </thead>
            <tbody>{this.showItems()}</tbody>
          </table>
          {this.showButtons()}
        </article>
        {this.showRecipes()}
      </section>
    );
  }
}
