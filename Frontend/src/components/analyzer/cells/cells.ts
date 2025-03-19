import StringCell from './StringCell.svelte';
import { createRender } from 'svelte-headless-table';
import RangeCell from './RangeCell.svelte';
import ListCell from './ListCell.svelte';
import FileCell from './FileCell.svelte';
import JsonCell from './JsonCell.svelte';
import type { AnalysisFieldType } from 'src/types/';
import BoolCell from './BoolCell.svelte';
import DateCell from './DateCell.svelte';
import UrlCell from './UrlCell.svelte';
import NumberCell from './NumberCell.svelte';

export const getCell = (type: AnalysisFieldType, subType: AnalysisFieldType | undefined = undefined) => {
    return ({ value }: any) => {

        if (value == undefined || value?.value == undefined) {
            return createRender(StringCell, { field: {
                value: ''
            } });
        }

        switch (type) {
            case 'Integer':
            case 'Float':
                return createRender(NumberCell, { field: value });
            case 'Boolean':
                return createRender(BoolCell, { field: value });
            case 'Range':
                return createRender(RangeCell, { field: value });
            case 'DateTime':
                return createRender(DateCell, { field: value });
            case 'URL':
                return createRender(UrlCell, { field: value });
            case 'Json':
                return createRender(JsonCell, { field: value });
            case 'File':
                return createRender(FileCell, { field: value });
            case 'List':
                return createRender(ListCell, { field: value, subType:subType! });
            default:
                return createRender(StringCell, { field: value });
        }
    }
};