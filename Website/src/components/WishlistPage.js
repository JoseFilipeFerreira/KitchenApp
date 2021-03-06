import React from "react";
import Swal from "sweetalert2";
import axios from "axios";

export default class WishlistList extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      categories: [],
      products: [],
    };
  }

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

  editWishlist = async () => {
    let token = localStorage.getItem("auth");
    let uid = this.props.wishlist_id;
    const form = new FormData();
    await Swal.fire({
      title: "Enter new name",
      input: "text",
      inputPlaceholder: "Enter your name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("uid", uid);
          form.append("name", value);

          axios
            .post("http://localhost:1331/wishlist/edit", form, {
              headers: { "Content-Type": "multipart/form-data", auth: token },
              withCredentials: true,
            })
            .then((response) => {
              /* save this token inside localStorage */
              const token = response.headers["auth"];
              localStorage.setItem("auth", token);
              this.props.handler();
            })
            .catch((error) => {
              console.log(error);
            });
        }
      },
    });
  };

  addProduct = async (product) => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    form.append("product", product["_guid"]);
    form.append("uid", this.props.wishlist_id);

    axios
      .post("http://localhost:1331/wishlist/addproduct", form, {
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
        Swal.fire("Nope!", "Product has not been added.", "error");
      });
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
        form.append("uid", this.props.wishlist_id);
        form.append("product", uid);

        axios
          .post("http://localhost:1331/wishlist/removeproduct", form, {
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

  shareWishlist = async () => {
    ;
    if (!this.props.shared) {
      let token = localStorage.getItem("auth");
      let uid = this.props.wishlist_id;
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
              .post("http://localhost:1331/wishlist/share", form, {
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
      Swal.fire("Nope!", "You are not the owner of this wishlist", "error");
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
            value="Edit wishlist"
            onClick={this.editWishlist}
          ></input>
          <input
            className="create-button"
            type="button"
            value="Share wishlist"
            onClick={this.shareWishlist}
          ></input>
        </div>
      );
    }
  };

  render() {
    return (
      <section className="grid">
        <article className="inventories">
          <div className="inventories-text">{this.props.wishlist_name}</div>
          <table id="inventoryList">
            <thead>
              <tr>
                <th>Name</th>
                <th></th>
              </tr>
            </thead>
            <tbody>{this.showItems()}</tbody>
          </table>
          {this.showButtons()}
        </article>
      </section>
    );
  }
}
