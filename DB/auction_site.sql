use auction_site;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL
);

-- Utworzenie tabeli items
CREATE TABLE items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10,2) NOT NULL,
    is_available TINYINT(1) DEFAULT 1,
    buyer_id INT,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (buyer_id) REFERENCES users(id)
);

CREATE TABLE auction_history (
    id INT AUTO_INCREMENT PRIMARY KEY,
    item_id INT,
    user_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10,2) NOT NULL,
    sold_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (item_id) REFERENCES items(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Wstawienie przykładowych danych do tabeli users
INSERT INTO users (username, password) VALUES
('test', '$2y$10$Qe/sNVJX5.us2oHGav648ed03TVGFzB1oQwQSv9IC2Nb4uvJlsEcG'), -- Hasło: test
('test_solder', '$2y$10$O7tboM8rFIiSWAVR8yXmP.ngSNZZMb5ef18drjTovll0fJG6T7TYy'); -- Hasło: test_solder

-- Wstawienie przykładowych danych do tabeli items
INSERT INTO items (user_id, title, description, price) VALUES
(1, 'Ksiazka XYZ', 'Opis ksiazki XYZ', 29.99),
(2, 'Laptop Dell', 'Opis laptopa Dell', 1599.99);