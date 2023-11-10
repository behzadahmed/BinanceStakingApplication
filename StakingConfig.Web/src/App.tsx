import logo from './logo.svg';
import './App.css';
import React, { useState, useEffect } from 'react';

type CryptoItem = {
    id: string;
    crypto: string;
};

function CryptoList() {
    const [items, setItems] = useState<CryptoItem[]>([]);
    const [newItem, setNewItem] = useState('');

    useEffect(() => {
        fetch('/api/CryptoList')
            .then(response => response.json())
            .then(data => setItems(data));
    }, []);

    function addItem() {
        fetch('/api/CryptoList', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ Crypto: newItem }),
        })
            .then(() => {
                fetch('/api/CryptoList')
                    .then(response => response.json())
                    .then(data => setItems(data));
            });
    }

    function deleteItem(id: string) {
        fetch(`/api/CryptoList/${id}`, {
            method: 'DELETE',
        })
            .then(() => {
                fetch('/api/CryptoList')
                    .then(response => response.json())
                    .then(data => setItems(data));
            });
    }

    return (
        <div>
            <input type="text" value={newItem} onChange={e => setNewItem(e.target.value)} />
            <button onClick={addItem}>Add Item</button>
            <ul>
                {items.map(item => (
                    <li key={item.id}>
                        {item.crypto}
                        <button onClick={() => deleteItem(item.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default CryptoList;