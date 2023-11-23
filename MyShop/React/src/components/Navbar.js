﻿import React from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
    return (
        <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div className="container-fluid">
                <Link className="navbar-brand text-light" to="/">AirDnD</Link>
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                    aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                 <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul className="navbar-nav flex-grow-1">
                        <li className="nav-item">
                            <Link className="nav-link text-light" to="/Test">Test1</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link text-light" to="/Test">Test2</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link text-light" to="/Test">Users</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link text-light" to="/BookingTable">Bookings</Link>
                        </li>
                    </ul>
                    {/* Implement _LoginPartial functionality*/}
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
