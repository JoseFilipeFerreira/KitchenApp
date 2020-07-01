import React from "react";
import Swal from "sweetalert2";
import axios from "axios";

export default class InventoryList extends React.Component {
  removeInventory = async (uid) => {
    let token = localStorage.getItem("auth");
    Swal.fire({
      title: "Remove Inventory",
      text: "Do you want to delete this inventory?",
      icon: "question",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.value) {
        axios
          .post(
            "http://localhost:1331/inventory/remove/" + uid,
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
            window.location.reload();
            Swal.fire("Deleted!", "Inventory has been deleted.", "success");
          })
          .catch((error) => {
            console.log(error);
            Swal.fire("Nope!", "Inventory has not been deleted.", "error");
          });
      }
    });
  };

  editInventory = async (uid) => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: name } = await Swal.fire({
      title: "Enter new inventory name",
      input: "text",
      inputPlaceholder: "Enter inventory name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("uid", uid);
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
              window.location.reload();
            })
            .catch((error) => {
              console.log(error);
              Swal.fire("Nope!", "Inventory has not been edited.", "error");
            });
        }
      },
    });
  };

  showInventoryList = function () {
    let names = [],
      ids = [];
    let json = this.props.inventories;
    for (let x in json) {
      names.push(x);
      ids.push(json[x]);
    }
    if (Object.keys(json).length) {
      return names.map((inventory, index) => {
        let link = "/dashboard/inventory/" + ids[index];
        return (
          <tr key={inventory + ids[index]}>
            <td key={ids[index]}>
              <a href={link}>{inventory}</a>
            </td>
            <td className="table-edit" key={"edit" + ids[index]}>
              <span
                onClick={() => {
                  this.editInventory(ids[index]);
                }}
              >
                ✏️
              </span>
            </td>
            <td className="table-edit" key={"remove" + ids[index]}>
              <span
                onClick={() => {
                  this.removeInventory(ids[index]);
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
      return names.map((inventory, index) => {
        let link = "/dashboard/shared/inventory/" + ids[index];
        return (
          <tr key={inventory + ids[index]}>
            <td key={ids[index]}>
              <a href={link}>{inventory}</a>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  createInventory = async () => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: name } = await Swal.fire({
      title: "Enter inventory name",
      input: "text",
      inputPlaceholder: "Enter inventory name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("name", value);

          axios
            .post("http://localhost:1331/inventory/add", form, {
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
      },
    });
  };

  render() {
    return (
      <section className="grid">
        <article className="inventories">
          <div className="inventories-text">Inventories</div>
          <table id="inventoryList">
            <thead />
            <tbody>{this.showInventoryList()}</tbody>
          </table>
          <div className="inventory-button">
            <input
              className="create-button"
              type="button"
              value="Create Inventory"
              onClick={this.createInventory}
            ></input>
          </div>
        </article>
        <article className="inventories">
          <div className="inventories-text">Shared Inventories</div>
          <table id="sharedList">
            <thead />
            <tbody>{this.showSharedList()}</tbody>
          </table>
        </article>
      </section>
    );
  }
}
