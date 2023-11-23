﻿import React, { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter, Routes, Route } from "react-router-dom"
import ControllerTest from "./ControllerTest"
import Test from "./Test";
import Layout from "./components/Layout";
import ItemListView from './components/ItemListViewModel';


function App() {
    return (
        <div className="App">
            <BrowserRouter>
                <Layout>
                    <Routes>
                        <Route path="/" element={<ControllerTest />} />
                        <Route exact path="/test" element={<Test />} />
                        <Route exact path="/items" element={<ItemListView />} />

                    </Routes>
                </Layout>
            </BrowserRouter>
        </div>
    )
}

export default App;
