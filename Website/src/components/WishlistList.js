import React from "react";
import Swal from "sweetalert2";
import axios from "axios";

export default class WishlistList extends React.Component {
  removeWishlist = async (uid) => {
    let token = localStorage.getItem("auth");
    Swal.fire({
      title: "Remove Wishlist",
      text: "Do you want to delete this wishlist?",
      icon: "question",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.value) {
        axios
          .post(
            "http://localhost:1331/wishlist/remove/" + uid,
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
            this.props.handler();
            Swal.fire("Deleted!", "Wishlist has been deleted.", "success");
          })
          .catch((error) => {
            console.log(error);
            Swal.fire("Nope!", "Wishlist has not been deleted.", "error");
          });
      }
    });
  };

  editWishlist = async (uid) => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    await Swal.fire({
      title: "Enter new wishlist name",
      input: "text",
      inputPlaceholder: "Enter wishlist name",
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
              Swal.fire("Edited!", "Wishlist has been edited.", "success");
            })
            .catch((error) => {
              console.log(error);
              Swal.fire("Nope!", "Wishlist has not been edited.", "error");
            });
        }
      },
    });
  };

  showWishlistList = function () {
    let names = [],
      ids = [];
    let json = this.props.wishlists;
    for (let x in json) {
      names.push(x);
      ids.push(json[x]);
    }
    if (Object.keys(json).length) {
      return names.map((wishlist, index) => {
        let link = "/dashboard/wishlist/" + ids[index];
        return (
          <tr key={wishlist + ids[index]}>
            <td key={ids[index]}>
              <a href={link}>{wishlist}</a>
            </td>
            <td className="table-edit" key={"edit" + ids[index]}>
              <span
                className="edit-button"
                role="img"
                aria-label="jsx-a11y/aria-proptypes"
                onClick={() => {
                  this.editWishlist(ids[index]);
                }}
              >
                ✏️
              </span>
            </td>
            <td className="table-edit" key={"remove" + ids[index]}>
              <span
                className="edit-button"
                role="img"
                aria-label="jsx-a11y/aria-proptypes"
                onClick={() => {
                  this.removeWishlist(ids[index]);
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
    console.log(this.props.shared);
    for (let x in json) {
      names.push(x);
      ids.push(json[x]);
    }
    if (Object.keys(json).length) {
      return names.map((wishlist, index) => {
        let link = "/dashboard/shared/wishlist/" + ids[index];
        return (
          <tr key={wishlist + ids[index]}>
            <td key={ids[index]}>
              <a href={link}>{wishlist}</a>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  createWishlist = async () => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    await Swal.fire({
      title: "Enter wishlist name",
      input: "text",
      inputPlaceholder: "Enter wishlist name",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("name", value);

          axios
            .post("http://localhost:1331/wishlist/add", form, {
              headers: { "Content-Type": "multipart/form-data", auth: token },
              withCredentials: true,
            })
            .then((response) => {
              /* save this token inside localStorage */
              const token = response.headers["auth"];
              localStorage.setItem("auth", token);
              this.props.handler();
              Swal.fire("Created!", "Wishlist has been created.", "success");
            })
            .catch((error) => {
              console.log(error);
              Swal.fire("Nope!", "Wishlist has not been created.", "error");
            });
        }
      },
    });
  };

  render() {
    return (
      <section className="grid">
        <article className="inventories">
          <div className="inventories-text">Wishlists</div>
          <table id="inventoryList">
            <thead />
            <tbody>{this.showWishlistList()}</tbody>
          </table>
          <div className="inventory-button">
            <input
              className="create-button"
              type="button"
              value="Create Wishlist"
              onClick={this.createWishlist}
            ></input>
          </div>
        </article>
        <article className="inventories">
          <div className="inventories-text">Shared Wishlists</div>
          <table id="sharedList">
            <thead />
            <tbody>{this.showSharedList()}</tbody>
          </table>
        </article>
      </section>
    );
  }
}
