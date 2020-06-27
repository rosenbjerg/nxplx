import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import { add, remove } from "../../utils/arrays";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { Library } from "../../utils/models";
import DirectoryBrowser from "../DirectoryBrowser";
import Loading from "../Loading";
import Modal from "../Modal";

interface Props {}
interface State {
    libraries: Library[]
    showDirectories: boolean
    createLibraryModalOpen: boolean
}

export default class LibraryManagement extends Component<Props, State> {
    public componentDidMount() {
        http.getJson<Library[]>('/api/library/list').then(libraries => this.setState({ libraries }));
    }
    public render(_, { libraries, showDirectories, createLibraryModalOpen }) {
        return (
            <div>
                {createLibraryModalOpen && (
                    <Modal onDismiss={() => this.setState({createLibraryModalOpen: false})}>
                        <h2>{translate('create-library')}</h2>
                        <form class="gapped" onSubmit={this.submitNewLibrary}>
                            <input class="inline-edit fullwidth gapped" name="name" placeholder={translate('name')}
                                   type="text" required/>
                            <select class="inline-edit fullwidth gapped" name="kind" required>
                                <option value={'film'}>{translate('film')}</option>
                                <option value={'series'}>{translate('series')}</option>
                            </select>
                            <select class="inline-edit fullwidth gapped" name="language" required>
                                <option value="en-UK">en-UK</option>
                                <option value="da-DK">da-DK</option>
                            </select>
                            <input class="inline-edit fullwidth gapped" name="path" placeholder={translate('path')} type="text" required/>
                            <button type="submit" class="material-icons bordered right">done</button>
                            <button type="button" class="bordered" onClick={() => this.setState({ showDirectories: !showDirectories })}>{translate(showDirectories ? 'hide-directories' : 'show-directories')}</button>
                        </form>
                        {showDirectories && (<DirectoryBrowser/>)}
                    </Modal>
                )}

                <table class="fullwidth">
                    <thead>
                        <tr>
                            <td>{translate('name')}</td>
                            <td>{translate('kind')}</td>
                            <td>{translate('language')}</td>
                            <td>{translate('path')}</td>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            !libraries ? (<Loading />) : libraries.map(lib => (
                                <tr key={lib.id}>
                                    <td>{lib.name}</td>
                                    <td>{translate(lib.kind)}</td>
                                    <td>{lib.language}</td>
                                    <td>{lib.path || translate('not specified')}</td>
                                    <td>
                                        <button type="button" onClick={this.indexLibrary(lib)} class="material-icons bordered">refresh</button>
                                        <button type="button" onClick={this.deleteLibrary(lib)} class="material-icons bordered">close</button>
                                    </td>
                                </tr>
                            ))
                        }
                    </tbody>
                </table>
                <button class="bordered" onClick={() => this.setState({ createLibraryModalOpen: true })}>{translate('create-library')}</button>
                <button class="bordered" onClick={this.indexAllLibraries}>{translate('index-all-libraries')}</button>
            </div>
        );
    }


    private indexLibrary = (library:Library) => async () => {
        const response = await http.post('/api/indexing', { value: [ library.id ] });
        const msg = response.ok ? `Indexing ${library.name}..` : 'Unable to start indexing :/';
        createSnackbar(msg, { timeout: 1500 });
    };
    private deleteLibrary = (library:Library) => async () => {
        const response = await http.delete('/api/library', { value: library.id });
        if (response.ok) this.setState({ libraries: remove(this.state.libraries, library) });
        const msg = response.ok ? `${library.name} deleted!` : 'Unable to remove the library :/';
        createSnackbar(msg, { timeout: 1500 });
    };
    private indexAllLibraries = async () => {
        const response = await http.post('/api/indexing', { value: this.state.libraries.map(lib => lib.id) });
        const msg = response.ok ? 'Indexing all libraries..' : 'Unable to start indexing :/';
        createSnackbar(msg, { timeout: 1500 });
    };
    private submitNewLibrary = async (ev:Event) => {
        ev.preventDefault();
        const formElement = ev.target as HTMLFormElement;
        const form = new FormData(formElement);
        const response = await http.post('/api/library', form, false);
        if (response.ok) {
            formElement.reset();
            this.setState({ libraries: add(this.state.libraries, await response.json()), createLibraryModalOpen: false });
        }
        else {
            createSnackbar('Unable to create new library :/', { timeout: 1500 });
        }
    };
}
