import React, { useState } from 'react';
import axios from 'axios';

const CopyDoc = () => {
    const [sourceDocumentName, setSourceDocumentName] = useState('');
    const [sourceFolderName, setSourceFolderName] = useState('');
    const [targetFolderName, setTargetFolderName] = useState('');
    const [copiedContent, setCopiedContent] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await axios.post('https://localhost:7289/api/CopyDoc/copy', null, {
                params: {
                    sourceFolderName,
                    sourceDocumentName,
                    targetFolderName,
                },
            });
           // console.log(response);
            console.log('Copy document response:', response.status);
            // Set copied content if successful
            setCopiedContent(response.data);
            setErrorMessage('');
        } catch (error) {
            console.error('Copy document error:', error);
            // Set error message if error occurs
            setErrorMessage('Error copying document. Please try again.');
            setCopiedContent('');
        }

        setSourceDocumentName('');
        setSourceFolderName('');
        setTargetFolderName('');
    };

    return (
        <div style={{ maxWidth: '500px', margin: 'auto', textAlign: 'justify' }}>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Source Document Name:
                        <input
                            type="text"
                            value={sourceDocumentName}
                            onChange={(e) => setSourceDocumentName(e.target.value)}
                        />
                    </label>
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Source Folder Name:
                        <input
                            type="text"
                            value={sourceFolderName}
                            onChange={(e) => setSourceFolderName(e.target.value)}
                        />
                    </label>
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <label>
                        Target Folder Name:
                        <input
                            type="text"
                            value={targetFolderName}
                            onChange={(e) => setTargetFolderName(e.target.value)}
                        />
                    </label>
                </div>
                <button type="submit" style={{ padding: '5px 10px', fontSize: '16px' }}>Copy Document</button>
            </form>
            {/* Display copied content or error message */}
            {copiedContent && <div style={{ marginTop: '20px' }}>Copied Content: {copiedContent}</div>}
            {errorMessage && <div style={{ marginTop: '20px', color: 'red' }}>Error: {errorMessage}</div>}
        </div>
    );
};

export default CopyDoc;
