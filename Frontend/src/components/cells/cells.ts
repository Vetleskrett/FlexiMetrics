import type { AnalyzerField } from 'src/types';
import StringCell from './StringCell.svelte';
import { createRender } from 'svelte-headless-table';
import RangeCell from './RangeCell.svelte';
import ListCell from './ListCell.svelte';
import FileCell from './FileCell.svelte';
import JsonCell from './JsonCell.svelte';

export const getCell = (field: AnalyzerField) => {
    switch (field.type) {
        case 'Range':
            return ({ value }: { value: any }) => createRender(RangeCell, { value, max: field.max });
        case 'List':
                return ({ value }: { value: any }) => createRender(ListCell, { values: value });
        case 'File':
            return ({ value }: { value: any }) => createRender(FileCell, { value: value });
        case 'Json':
            return ({ value }: { value: any }) => createRender(JsonCell, { value: value });
        default:
            return ({ value }: { value: any }) => createRender(StringCell, { value });
    }
};