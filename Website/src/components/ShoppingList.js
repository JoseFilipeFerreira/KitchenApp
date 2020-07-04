import React from "react";
import Swal from "sweetalert2";
import axios from "axios";

export default class ShoppingList extends React.Component {
  removeShopping = async (uid) => {
    let token = localStorage.getItem("auth");
    Swal.fire({
      title: "Remove Shopping list",
      text: "Do you want to delete this shopping list?",
      icon: "question",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.value) {
        axios
          .post(
            "http://localhost:1331/shopping/remove/" + uid,
            {},
            {
              headers: { auth: token },
              withCredentials: true,
            }
          )
          .then((response) => {
            /* save this token inside localStorage */
            const token = response.headers["auth"];
            localStorage.setItem("auth", token);
            this.props.handler()
            Swal.fire("Deleted!", "Shopping list has been deleted.", "success");
          })
          .catch((error) => {
            console.log(error);
            Swal.fire("Nope!", "Shopping list has not been deleted.", "error");
          });
      }
    });
  };

  editShopping = async (uid) => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: name } = await Swal.fire({
      title: "Enter new shopping list name",
      input: "text",
      inputPlaceholder: "Enter shopping list name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("uid", uid);
          form.append("name", value);

          axios
            .post("http://localhost:1331/shopping/edit", form, {
              headers: { "Content-Type": "multipart/form-data", auth: token },
              withCredentials: true,
            })
            .then((response) => {
              /* save this token inside localStorage */
              const token = response.headers["auth"];
              localStorage.setItem("auth", token);
              this.props.handler()
              Swal.fire("Edited!", "Shopping list has been edited.", "success");
            })
            .catch((error) => {
              console.log(error);
              Swal.fire("Nope!", "Shopping list has not been edited.", "error");
            });
        }
      },
    });
  };

  showShoppingList = function () {
    let names = [],
      ids = [];
    let json = this.props.shoppings;
    for (let x in json) {
      names.push(x);
      ids.push(json[x]);
    }
    if (Object.keys(json).length) {
      return names.map((shopping, index) => {
        let link = "/dashboard/shopping/" + ids[index];
        return (
          <tr key={shopping + ids[index]}>
            <td key={ids[index]}>
              <a href={link}>{shopping}</a>
            </td>
            <td className="table-edit" key={"edit" + ids[index]}>
              <span
                onClick={() => {
                  this.editShopping(ids[index]);
                }}
              >
                ✏️
              </span>
            </td>
            <td className="table-edit" key={"remove" + ids[index]}>
              <span
                onClick={() => {
                  this.removeShopping(ids[index]);
                }}
              >
                ❌
              </span>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  showSharedList = function () {
    let names = [],
      ids = [];
    let json = this.props.shared;
    console.log(this.props.shared)
    for (let x in json) {
      names.push(x);
      ids.push(json[x]);
    }
    if (Object.keys(json).length) {
      return names.map((shopping, index) => {
        let link = "/dashboard/shared/shopping/" + ids[index];
        return (
          <tr key={shopping + ids[index]}>
            <td key={ids[index]}>
              <a href={link}>{shopping}</a>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  createShopping = async () => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: name } = await Swal.fire({
      title: "Enter shopping list name",
      input: "text",
      inputPlaceholder: "Enter shopping list name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("name", value);

          axios
            .post("http://localhost:1331/shopping/add", form, {
              headers: { "Content-Type": "multipart/form-data", auth: token },
              withCredentials: true,
            })
            .then((response) => {
              /* save this token inside localStorage */
              const token = response.headers["auth"];
              localStorage.setItem("auth", token);
              this.props.handler()
              Swal.fire("Created!", "Shopping list has been created.", "success");
            })
            .catch((error) => {
              console.log(error);
              Swal.fire("Nope!", "Shopping list has not been created.", "error");
            });
        }
      },
    });
  };

  render() {
    return (
      <section className="grid">
        <article className="inventories">
          <div className="inventories-text">Shopping lists</div>
          <table id="inventoryList">
            <thead />
            <tbody>{this.showShoppingList()}</tbody>
          </table>
          <div className="inventory-button">
            <input
              className="create-button"
              type="button"
              value="Create Shopping"
              onClick={this.createShopping}
            ></input>
          </div>
        </article>
        <article className="inventories">
          <div className="inventories-text">Shared Shopping lists</div>
          <table id="sharedList">
            <thead />
            <tbody>{this.showSharedList()}</tbody>
          </table>
        </article>
      </section>
    );
  }
}
