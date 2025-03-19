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
    switch (type) {
        case 'Integer':
        case 'Float':
            return ({ value }: { value: any }) => createRender(NumberCell, { value });
        case 'Boolean':
            return ({ value }: { value: any }) => createRender(BoolCell, { value });
        case 'Range':
            return ({ value }: { value: any }) => createRender(RangeCell, { value });
        case 'DateTime':
            return ({ value }: { value: any }) => createRender(DateCell, { value });
        case 'URL':
            return ({ value }: { value: any }) => createRender(UrlCell, { value });
        case 'Json':
            return ({ value }: { value: any }) => createRender(JsonCell, { value });
        case 'List':
            return ({ value }: { value: any }) => createRender(ListCell, { value, subType:subType! });
        default:
            return ({ value }: { value: any }) => createRender(StringCell, { value });
    }
};