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

export const getCell = (type: AnalysisFieldType) => {
    switch (type) {
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
        default:
            return ({ value }: { value: any }) => createRender(StringCell, { value });
    }
};