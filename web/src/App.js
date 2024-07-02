import React, { useState, createContext, useContext } from 'react';
import { BrowserRouter as Router, Route, Routes, Link, useNavigate } from 'react-router-dom';
import ItemTable from './ItemTable';
import EditItem from './EditItem';
import Login from './Login';
import Register from './Register';
import MyItemTable from './MyItems';
import AddItem from './AddItem';
import DetailsItem from './DetailsItem';
import './App.css';
import useLoginTimeout from './loginTimeout';
import ExchangeRatesProvider from './ExchangeRatesProvider';
import { Helmet } from 'react-helmet';
import MapComponent from './MapComponent';

export const CurrencyContext = createContext(); // Eksportujemy CurrencyContext

function useCurrency() {
  return useContext(CurrencyContext);
}

function Navigation() {
  const username = localStorage.getItem('username');
  const navigate = useNavigate();
  const { clearLoginTimeout } = useLoginTimeout();
  document.title = "Advertising portal";
  const [selectedCurrency, setSelectedCurrency] = useCurrency();

  const handleCurrencyChange = (e) => {
    setSelectedCurrency(e.target.value);
    console.log('Selected currency:', e.target.value);
  };

  const handleLogout = () => {
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    clearLoginTimeout();
    navigate('/login');
  };

  return (
    <nav className='nav-container'>
      {username ? (
        <span className="hi">Hi, {username}</span>
      ) : (
        <span className="hi">Hi</span>
      )}

      <ul>
        <li>
          <Link to="/" className="nav-item">All Items</Link>
        </li>

        {username ? (
          <>
            <li>
              <Link to="/myitems" className="nav-item">My items</Link>
            </li>
            <li>
              <button onClick={handleLogout} className="nav-item">Logout</button>
            </li>
          </>
        ) : (
          <>
            <li>
              <Link to="/login" className="nav-item">Login</Link>
            </li>
            <li>
              <Link to="/register" className="nav-item">Register</Link>
            </li>
          </>
        )}
        <li>
          <select value={selectedCurrency} onChange={handleCurrencyChange} className="nav-item">
            <option value="PLN">PLN</option>
            <option value="EUR">EUR</option>
            <option value="USD">USD</option>
            <option value="GBP">GBP</option>
          </select>
        </li>
      </ul>
    </nav>
  );
}

function App() {
  const [selectedCurrency, setSelectedCurrency] = useState('PLN');

  return (
    <ExchangeRatesProvider>
      <CurrencyContext.Provider value={[selectedCurrency, setSelectedCurrency]}>
        <Router>
          <div>
            <Helmet>
              <link rel="stylesheet" href="https://unpkg.com/leaflet/dist/leaflet.css" />
            </Helmet>
            <Navigation />
            <Routes>
              <Route path="/" element={<ItemTable />} />
              <Route path="/edit/:id" element={<EditItem />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/myitems" element={<MyItemTable />} />
              <Route path="/additem" element={<AddItem />} />
              <Route path="/details/:id" element={<DetailsItem />} />
              <Route path="/map/:city/:street" element={<MapComponent/>} /> {/* Przekaż miasto i ulicę */}
            </Routes>
          </div>
        </Router>
      </CurrencyContext.Provider>
    </ExchangeRatesProvider>
  );
}

export default App;
