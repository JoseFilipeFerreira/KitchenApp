import React from "react";
import Swal from "sweetalert2";
import axios from "axios";
import RecipesTable from "./RecipesTable";

export default class InventoryList extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      recipes: [],
      categories: [],
      products: [],
    };
  }

  validateBirthday = (e) => {
    if (
      /^([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$/.test(e) &&
      e !== ""
    ) {
      return true;
    } else return false;
  };

  addProductMenu = () => {
    let token = localStorage.getItem("auth");
    axios
      .get(
        "http://localhost:1331/product/category/all",
        {
          headers: { auth: token },
        },
        { withCredentials: true }
      )
      .then((response) => {
        let json = response.data;
        this.setState({
          categories: json,
        });
        this.chooseCategory();
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
      })
      .catch((error) => {
        console.log(error);
      });
  };

  chooseCategory = async () => {
    const { value: category } = await Swal.fire({
      title: "Select category",
      input: "select",
      inputOptions: this.state.categories,
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

    if (category) this.showProductsList(this.state.categories[category]);
  };

  showProductsList = (category) => {
    let token = localStorage.getItem("auth");
    let form = new FormData();
    form.append("category", category);
    axios
      .post("http://localhost:1331/product/category/getprods", form, {
        headers: { "Content-Type": "multipart/form-data", auth: token },
        withCredentials: true,
      })
      .then((response) => {
        let json = response.data;
        this.setState({
          products: json,
        });
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
        this.chooseProduct();
      })
      .catch((error) => {
        console.log(error);
      });
  };

  chooseProduct = async () => {
    let names = this.state.products.map((x) => x._name);
    ;
    const { value: product } = await Swal.fire({
      title: "Select product",
      input: "select",
      inputOptions: names,
      inputPlaceholder: "Select a product",
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

    if (product) this.addProduct(this.state.products[product]);
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
          this.props.handler();
          Swal.fire("Product Added!", "Product has been added.", "success");
        })
        .catch((error) => {
          console.log(error);
          Swal.fire(
            "Nope!",
            "Product has not been added. Make sure to insert valid expire date and valid quantity.",
            "error"
          );
        });
    }
  };

  removeProduct = async (uid) => {
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
            this.props.handler();
            Swal.fire(
              "Product Removed!",
              "Product has been removed.",
              "success"
            );
          })
          .catch((error) => {
            console.log(error);
            Swal.fire("Nope!", "Product has not been removed.", "error");
          });
      }
    });
  };

  editProduct = async (uid) => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: formValues } = await Swal.fire({
      title: "Edit Product",
      html:
        '<input id="swal-input1" placeholder="Quantity" class="swal2-input">' +
        '<input id="swal-input2" placeholder="Expire date (YYYY/MM/DD)" class="swal2-input">' +
        "(You can edit only one, leave the other blank)",
      focusConfirm: false,
      preConfirm: () => {
        let quantity = document.getElementById("swal-input1").value;
        let expire = document.getElementById("swal-input2").value;
        return [quantity, expire];
      },
    });

    if (formValues != null && formValues[0] != null && formValues[1] != null) {
      form.append("product", uid);
      if (formValues[0]) form.append("quantity", formValues[0]);
      if (formValues[1]) form.append("expire", formValues[1]);
      form.append("uid", this.props.inventory_id);

      axios
        .post("http://localhost:1331/inventory/editproduct", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token },
          withCredentials: true,
        })
        .then((response) => {
          /* save this token inside localStorage */
          const token = response.headers["auth"];
          localStorage.setItem("auth", token);
          this.props.handler();
          Swal.fire("Product edited!", "Product has been edited.", "success");
        })
        .catch((error) => {
          console.log(error);
          Swal.fire(
            "Nope!",
            "Product has not been edited. Make sure to insert valid expire date and/or valid quantity",
            "error"
          );
        });
    }
  };

  showItems = () => {
    let json = this.props.items;
    json.sort(function (a, b) {
      return a._name.localeCompare(b._name);
    });
    return json.map((item, index) => {
      ;
      return (
        <tr>
          <td>{item._name}</td>
          <td>{item._stock}</td>
          <td>{item._consume_before.substring(0, 10)}</td>
          <td className="table-edit" key={"edit" + item._guid}>
            <span
              className="edit-button"
              role="img"
              aria-label="jsx-a11y/aria-proptypes"
              onClick={() => {
                this.editProduct(item._guid);
              }}
            >
              ✏️
            </span>
          </td>
          <td className="table-edit" key={"remove" + item._guid}>
            <span
              className="edit-button"
              role="img"
              aria-label="jsx-a11y/aria-proptypes"
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
        })
        .catch((error) => {
          console.log(error);
        });
    } else {
      Swal.fire("Nope!", "You don't have items on this inventory", "error");
    }
  };

  editInventory = async (uid) => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    await Swal.fire({
      title: "Enter new inventory name",
      input: "text",
      inputPlaceholder: "Enter inventory name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("uid", this.props.inventory_id);
          form.append("name", value);

          axios
            .post("http://localhost:1331/inventory/edit", form, {
              headers: { "Content-Type": "multipart/form-data", auth: token },
              withCredentials: true,
            })
            .then((response) => {
              /* save this token inside localStorage */
              const token = response.headers["auth"];
              localStorage.setItem("auth", token);
              this.props.handler();
              Swal.fire("Edited!", "Inventory has been edited.", "success");
            })
            .catch((error) => {
              console.log(error);
              Swal.fire("Nope!", "Inventory has not been edited.", "error");
            });
        }
      },
    });
  };

  shareInventory = async () => {
    ;
    if (!this.props.shared) {
      let token = localStorage.getItem("auth");
      let uid = this.props.inventory_id;
      const form = new FormData();
      await Swal.fire({
        title: "Enter user email",
        input: "email",
        inputPlaceholder: "Enter email",
        inputValidator: (value) => {
          if (!value) {
            return "Invalid Name";
          } else {
            form.append("uid", uid);
            form.append("friend", value);

            axios
              .post("http://localhost:1331/inventory/share", form, {
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
                Swal.fire(
                  "Nope!",
                  "User has not been added. Check if email exists.",
                  "error"
                );
              });
          }
        },
      });
    } else {
      Swal.fire(
        "Nope!",
        "You are not the owner of this shopping list",
        "error"
      );
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
          <RecipesTable data={this.state.recipes} stared={false} />
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
