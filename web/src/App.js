import logo from './logo.svg';
import './App.css';
import ItemList from './ListAllAvailableItems';

function App() {
  return (
    <div className="App">
      
      <main>
      <ItemList /> {/* Dodaj komponent ItemList do renderowanego drzewa komponentów */}
      </main>
    </div>
  );
}

export default App;
